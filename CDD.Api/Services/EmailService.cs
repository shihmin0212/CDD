using FluentEmail.Core;
using FluentEmail.Core.Models;
using Newtonsoft.Json;
using Sample.Api.Extensions;
using Sample.Api.Models.EmailService;

namespace Sample.Api.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailMetaData emailMetadata, bool isHtml = false);

        void LogNotify(LogLevel logLevel, string title, object? contenet);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;

        private readonly IConfiguration _config;

        private readonly IFluentEmailFactory _fluentEmail;

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1); // 最多允許 1 個並發發送

        public EmailService(ILogger<EmailService> logger, IConfiguration config, IFluentEmailFactory fluentEmail)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _fluentEmail = fluentEmail ?? throw new ArgumentNullException(nameof(fluentEmail));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailMetadata"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public async Task SendAsync(EmailMetaData emailMetadata, bool isHtml = false)
        {
            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token; // 5 秒後自動取消
                await _fluentEmail.Create().To(emailMetadata.ToAddress)
            .CC(emailMetadata.CCToAddress)
            .Subject(emailMetadata.Subject)
            .Body(emailMetadata.Body, isHtml)
            .SendAsync(cts);

                _logger.LogInformation($"{emailMetadata.ToAddress}: ${emailMetadata.Subject} : 已寄送");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message + ex.StackTrace}");
            }
        }

        public void LogNotify(LogLevel logLevel, string title, object? contenet)
        {
            try
            {
                string message = (contenet != null) ? JsonConvert.SerializeObject(contenet, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }) : String.Empty;
                // mail Target 
                string? _mailList = _config.GetValue("MailList", String.Empty);
                string[]? _mailListArray = (!String.IsNullOrWhiteSpace(_mailList)) ? _mailList.Split(",") : null;
                List<Address> _mailAddressesList = new List<Address>();
                if (_mailListArray != null && _mailListArray.Any())
                {
                    foreach (string _mail in _mailListArray)
                    {
                        if (!_mail.IsValidEmailFormat())
                        {
                            _logger.LogError($"mailCC Format Error【{_mail}】");
                        }
                        else
                        {
                            _mailAddressesList.Add(new Address(_mail));
                        }
                    }
                }


                //mail CC 
                string? _mailCCList = _config.GetValue("MailCCList", String.Empty);
                string[]? _mailCCListArray = (!String.IsNullOrWhiteSpace(_mailCCList)) ? _mailCCList.Split(",") : null;
                List<Address> _mailCCAddressesList = new List<Address>();
                if (_mailCCListArray != null && _mailCCListArray.Any())
                {
                    foreach (string _mailCC in _mailCCListArray)
                    {
                        if (!_mailCC.IsValidEmailFormat())
                        {
                            _logger.LogError($"mailCC Format Error【{_mailCC}】");
                        }
                        else
                        {
                            _mailCCAddressesList.Add(new Address(_mailCC));
                        }
                    }
                }

                EmailMetaData emailMetaData = new EmailMetaData(
                _mailAddressesList,
                _mailCCAddressesList,
                $"【{logLevel}】【{System.Net.Dns.GetHostName()}】【{title}】",
                $"Msg:{contenet} \n " +
                $"DateTime: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")}"
                );
                SendEmailControlledAsync(emailMetaData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{DateTime.Now:HH:mm:ss} - Error; {JsonConvert.SerializeObject(ex)}");
            }

        }

        /// <summary>
        /// 控制並發數量 1 
        /// </summary>
        /// <param name="emailMetadata"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public async Task SendEmailControlledAsync(EmailMetaData emailMetadata, bool isHtml = false)
        {
            await _semaphore.WaitAsync();
            try
            {
                await SendAsync(emailMetadata, isHtml);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

}