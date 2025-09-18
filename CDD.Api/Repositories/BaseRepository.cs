using CDD.Api.Helpers;

namespace CDD.Api.Repositories
{
    public class BaseRepository<T>
    {
        protected readonly IConfiguration _config = null!;
        protected readonly ILogger<T> _logger = null!;
        protected readonly IConfigurationSection _storedProcedures = null!;
        protected readonly IDapperHelper _dapperHelper = null!;

        public BaseRepository(IConfiguration config, ILogger<T> logger, IDapperHelper dapperHelper)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(_config));
            this._logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            this._storedProcedures = this._config.GetSection("StoredProcedures") ?? throw new ArgumentNullException("StoredProcedures not found in appsetting");
            this._dapperHelper = dapperHelper ?? throw new ArgumentNullException(nameof(_dapperHelper));
        }
    }
}
