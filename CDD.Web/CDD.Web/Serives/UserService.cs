using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using ITfoxtec.Identity.Saml2;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CDD.Web.Extensions;
using CDD.Web.Helpers;
using CDD.Web.Libs;
using CDD.Web.Models.DTO;
using CDD.Web.Models.Request;
using CDD.Web.Models.Response;
using CDD.Web.Serives;

namespace CDD.Web.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private HttpContext HttpContext
        {
            get
            {
                return _contextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(HttpContext));
            }
        }

        private readonly IWebHostEnvironment _env;

        private readonly ILogger<UserService> _logger;

        private readonly IConfiguration _config;

        private readonly APIHelper _apiHelper;

        private readonly IIPHelper _ipHelper;

        private readonly ApiKeyService _apiKeyService;

        private readonly ExternalSystem _systemInfoForApiService;

        private readonly RsaKeyOptions _rsaKeyOptions;

        private ConfigurationSection _Saml2Setting;

        private readonly IRequest _request;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="apiHelper"></param>
        /// <param name="ipHelper"></param>
        /// <param name="rsaKeyOptions"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserService(
           IHttpContextAccessor accessor,
           IWebHostEnvironment env,
           ILogger<UserService> logger,
           IConfiguration config,
           APIHelper apiHelper,
           IIPHelper ipHelper,
           ApiKeyService apiKeyService,
           IOptions<RsaKeyOptions> rsaKeyOptions,
           Libs.IRequest request
           )
        {
            this._contextAccessor = accessor;
            this._env = env ?? throw new ArgumentNullException(nameof(env));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._config = config ?? throw new ArgumentNullException(nameof(config));

            this._apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
            this._ipHelper = ipHelper ?? throw new ArgumentNullException(nameof(ipHelper));
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _systemInfoForApiService = _apiKeyService.GetSystemInfoForApiService();
            // Fix the error by accessing the Value property of rsaKeyOptions
            _rsaKeyOptions = rsaKeyOptions?.Value ?? throw new ArgumentNullException(nameof(rsaKeyOptions));

            // _Web Setting 
            _Saml2Setting = (ConfigurationSection)config.GetSection("Saml2") ?? throw new ArgumentNullException("appsetting::Saml2");
            _request = request ?? throw new ArgumentNullException(nameof(request));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RsaKeyOptions GerRsaKeys()
        {
            if (_rsaKeyOptions == null)
            {
                throw new ArgumentNullException(nameof(_rsaKeyOptions), "RSA Key Options is not initialized.");
            }
            RsaKeyOptions rsaKey = new RsaKeyOptions
            {
                RsaPublicKeyXml = Encoding.UTF8.GetString(Convert.FromBase64String(_rsaKeyOptions.RsaPublicKeyXml)),
                RsaPrivateKeyXml = Encoding.UTF8.GetString(Convert.FromBase64String(_rsaKeyOptions.RsaPrivateKeyXml))
            };
            return rsaKey;
        }

        /// <summary>
        /// RSA PEM 格式
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string GetRsaPublicKeyPem()
        {
            if (_rsaKeyOptions == null)
            {
                throw new ArgumentNullException(nameof(_rsaKeyOptions), "RSA Key Options is not initialized.");
            }
            string RsaPublicKeyXml = Encoding.UTF8.GetString(Convert.FromBase64String(_rsaKeyOptions.RsaPublicKeyXml));
            return ConvertXmlToPem(RsaPublicKeyXml);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string ConvertXmlToPem(string xml)
        {
            var rsa = RSA.Create();
            rsa.FromXmlString(xml);

            var publicKey = rsa.ExportSubjectPublicKeyInfo();
            var pem = new StringBuilder();
            pem.AppendLine("-----BEGIN PUBLIC KEY-----");
            pem.AppendLine(Convert.ToBase64String(publicKey, Base64FormattingOptions.InsertLineBreaks));
            pem.AppendLine("-----END PUBLIC KEY-----");
            return pem.ToString();
        }

        /// <summary>
        /// 使用 RSA 私鑰解密 AES 金鑰（Base64）
        /// </summary>
        public string DecryptRsa(string encryptedBase64)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(Encoding.UTF8.GetString(Convert.FromBase64String(_rsaKeyOptions.RsaPrivateKeyXml)));
                byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, false);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        /// <summary>
        /// 使用 AES 解密帳號密碼資料（Base64 AES 密文 + HEX 金鑰 + Base64 IV）
        /// </summary>
        public string DecryptAes(string encryptedBase64, string keyHex, string ivBase64)
        {
            byte[] keyBytes = Convert.FromHexString(keyHex);
            byte[] ivBytes = Convert.FromBase64String(ivBase64);
            byte[] cipherBytes = Convert.FromBase64String(encryptedBase64);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                using (MemoryStream ms = new MemoryStream(cipherBytes))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        /// <summary>
        /// 解密登入請求物件，傳回帳號密碼實體
        /// </summary>
        public LoginData? DecryptLoginData(LoginPostBackReq req)
        {
            string aesKey = DecryptRsa(req.EncryptedAesKey);
            string json = DecryptAes(req.AesEncryptedData, aesKey, req.Base64Iv);
            return JsonConvert.DeserializeObject<LoginData>(json);
        }

        /// <summary>
        /// 登入處理
        /// 前端流程（JavaScript）
        /// 隨機產生一組 AES 金鑰
        /// 用 AES 加密使用者輸入的帳號與密碼
        /// 用伺服器提供的 RSA 公鑰 加密 AES 金鑰
        /// 將「AES 加密帳密」與「RSA 加密的 AES 金鑰」送到後端
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<GeneralResp> Login(LoginPostBackReq req)
        {
            GeneralResp resp = new GeneralResp() { Status = false };
            LoginData? loginData = DecryptLoginData(req);
            if (loginData == null) { throw MyExceptionList.Unauthorized().GetException(); }

            LoginData postData = new LoginData
            {
                Account = CommonUtilities.Helpers.EncryptionHelper.EncryptString(loginData.Account, _systemInfoForApiService.HashKey, _systemInfoForApiService.IVKey),
                Password = CommonUtilities.Helpers.EncryptionHelper.EncryptString(loginData.Password, _systemInfoForApiService.HashKey, _systemInfoForApiService.IVKey)
            };
            bool isLogin = await _apiHelper.VerifyAdminUser(postData);
            if (isLogin)
            {
                UserLoginData userLoginData = new UserLoginData
                {
                    UserName = loginData.Account,
                    LoginTime = DateTime.UtcNow,
                    IPAddress = _ipHelper.GetUserIP(),
                };
                HttpContext.Session.SetObject(SessionKey.UserLoginData, userLoginData);
            }
            resp.Status = isLogin;
            return resp;
        }

        /// <summary>
        /// 檢查是否登入
        /// </summary>
        /// <returns></returns>
        public GeneralResp CheckIfLogin()
        {
            try
            {
                UserLoginData? userLoginData = HttpContext.Session.GetObject<UserLoginData>(SessionKey.UserLoginData);
                return new GeneralResp() { Status = userLoginData != null };
            }
            catch (Exception ex)
            {
                throw MyExceptionList.SessionExpired(JsonConvert.SerializeObject(ex)).GetException();
            }
        }

        /// <summary>
        /// 是否登入
        /// </summary>
        /// <returns></returns>
        public bool IsLogin()
        {
            UserLoginData? userLoginData = HttpContext.Session.GetObject<UserLoginData>(SessionKey.UserLoginData);
            if (userLoginData != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void Logout()
        {
            HttpContext.Session.Clear();
        }

        /// <summary>
        /// ACS：接收和驗證 ADFS 回傳的 SAML Response
        /// </summary>
        public async Task<bool> LoginBySaml2()
        {
            try
            {
                // 1. 驗證 HTTP 方法
                if (!string.Equals(HttpContext.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("SAML ACS 端點只接受 POST 方法");
                    return false;
                }

                // 2. 建立基本 SAML2 配置並驗證必要參數
                var issuer = _Saml2Setting["Issuer"];
                var audienceUri = _Saml2Setting["AudienceUri"];

                if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audienceUri))
                {
                    _logger.LogError("SAML2 配置不完整：Issuer 或 AudienceUri 為空");
                    return false;
                }

                var saml2Config = new Saml2Configuration
                {
                    Issuer = issuer
                };
                saml2Config.AllowedAudienceUris.Add(audienceUri);

                // 3. 從 HTTP Request 讀取並驗證 SAML Response
                string? samlResponse = HttpContext.Request.Form["SAMLResponse"].ToString();
                if (string.IsNullOrWhiteSpace(samlResponse))
                {
                    _logger.LogWarning("未找到有效的 SAMLResponse 參數");
                    return false;
                }

                // 4. 驗證 SAML Response 長度（防止過大的請求）
                if (samlResponse.Length > 100000) // 100KB 限制
                {
                    _logger.LogWarning("SAMLResponse 過大，可能的攻擊嘗試");
                    return false;
                }

                // 5. 使用 POST binding 處理 SAML Response

                var binding = new Saml2PostBinding();
                var saml2AuthnResponse = new Saml2AuthnResponse(saml2Config);

                // 6. 建立符合 ITfoxtec 要求的 HttpRequest
                var formCollection = new NameValueCollection();
                formCollection.Add("SAMLResponse", samlResponse);

                var httpRequest = new ITfoxtec.Identity.Saml2.Http.HttpRequest
                {
                    Form = formCollection
                };

                // 7. 讀取和驗證 SAML Response
                binding.ReadSamlResponse(httpRequest, saml2AuthnResponse);

                // 8. 驗證 SAML Response 狀態
                if (saml2AuthnResponse.Status != ITfoxtec.Identity.Saml2.Schemas.Saml2StatusCodes.Success)
                {
                    _logger.LogWarning($"SAML Response 狀態錯誤: {saml2AuthnResponse.Status}");
                    return false;
                }

                // 9. 驗證 Issuer 是否匹配
                if (!string.Equals(saml2AuthnResponse.Issuer, issuer, StringComparison.Ordinal))
                {
                    _logger.LogWarning($"SAML Response Issuer 不匹配. 期望: {issuer}, 實際: {saml2AuthnResponse.Issuer}");
                    return false;
                }

                // 10. 取得和驗證使用者資訊
                var claimsIdentity = saml2AuthnResponse.ClaimsIdentity;
                if (claimsIdentity?.IsAuthenticated != true)
                {
                    _logger.LogWarning("SAML Response 未包含有效的認證身份");
                    return false;
                }

                // 11. 提取使用者資訊（優先使用 EmployeeID）
                var employeeId = claimsIdentity.Claims.FirstOrDefault(c => string.Equals(c.Type, "EmployeeID", StringComparison.Ordinal))?.Value;

                // 12. 驗證 EmployeeID 格式（假設應為數字）
                if (string.IsNullOrWhiteSpace(employeeId) || !employeeId.All(char.IsDigit))
                {
                    _logger.LogWarning($"無效的 EmployeeID: {employeeId}");
                    return false;
                }

                // 13. 建立使用者登入資料
                var userLoginData = new UserLoginData
                {
                    UserName = employeeId, // 使用 EmployeeID 作為主要識別
                    LoginTime = DateTime.UtcNow,
                    IPAddress = _ipHelper.GetUserIP(),
                };

                // 14. 清除舊的 Session 並設定新的登入資料
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("EmployeeID", employeeId);
                HttpContext.Session.SetObject(SessionKey.UserLoginData, userLoginData);

                _logger.LogInformation($"ADFS 使用者登入成功 - EmployeeID: {employeeId}, IP: {userLoginData.IPAddress}");
                return true;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "SAML Response 參數驗證失敗");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ADFS 登入處理失敗");
                return false;
            }
        }

    }
}
