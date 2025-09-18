using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Sample.Api.Helpers
{
    public interface IMemoryCacheHelper
    {
        Task AddAsync(object key, object value, MemoryCacheEntryOptions? memoryCacheEntryOptions = null);

        void Add(object key, object value, MemoryCacheEntryOptions? memoryCacheEntryOptions = null);

        T? GetValue<T>(string key);

        Task<T?> GetValueAsync<T>(string key);

        void Remove(string key);

        void Clear();
    }

    public class MemoryCacheHelper : IMemoryCacheHelper
    {
        private readonly IWebHostEnvironment _env;

        private readonly ILogger<IMemoryCacheHelper> _logger;

        private readonly IConfiguration _config;

        private readonly IMemoryCache _memoryCache;

        private CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly ConfigurationSection _WebSetting;

        public MemoryCacheHelper(

            IHttpContextAccessor accessor,
            IWebHostEnvironment env,
            ILogger<IMemoryCacheHelper> logger,
            IConfiguration config,
            IMemoryCache memoryCache
            )
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _WebSetting = (ConfigurationSection)_config.GetSection("WebSetting") ?? throw new ArgumentNullException("WebSetting");
        }


        public async Task AddAsync(object key, object value, MemoryCacheEntryOptions? memoryCacheEntryOptions = null)
        {
            try
            {
                await _semaphore.WaitAsync();

                Add(key, value, memoryCacheEntryOptions);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public void Add(object key, object value, MemoryCacheEntryOptions? memoryCacheEntryOptions = null)
        {
            if (memoryCacheEntryOptions == null)
            {
                int cacheSlidingExpirationMin = _WebSetting.GetValue("CacheSlidingExpirationMin", 60 * 24);
                int cacheAbsoluteExpirationMin = _WebSetting.GetValue("CacheAbsoluteExpirationMin", 60 * 24);

                memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(cacheSlidingExpirationMin))             // 會設定為滑動到期 60 秒。 如果快取專案未存取超過60秒，則會從快取中收回
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheAbsoluteExpirationMin))           // 絕對過期時間：時間到後就會消失
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1024);
            }
            using (ICacheEntry entry = _memoryCache.CreateEntry(key))
            {
                entry.SetOptions(memoryCacheEntryOptions);
                entry.Value = value;
                // add an expiration token that allows us to clear the entire cache with a single method call
                entry.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
            }
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public void Clear()
        {
            _resetCacheToken.Cancel(); // this triggers the CancellationChangeToken to expire every item from cache
            _resetCacheToken.Dispose(); // dispose the current cancellation token source and create a new one
            _resetCacheToken = new CancellationTokenSource();
        }

        public T? GetValue<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public async Task<T?> GetValueAsync<T>(string key)
        {
            T? value;
            if (_memoryCache.TryGetValue(key, out value))
            {
                _logger.LogInformation($"{key} found in cache.");
                return value;
            }
            else
            {
                try
                {
                    await _semaphore.WaitAsync();

                    if (_memoryCache.TryGetValue(key, out value))
                    {
                        _logger.LogInformation($"{key} found in cache.(2)");
                        return value;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            return value;
        }
    }
}
