using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CDD.Api.Attributes;
using CDD.Api.Helpers;
using CDD.Api.Libs;
using CDD.Api.Models.Request;
using CDD.Api.Models.Request.Admin;
using CDD.Api.Models.Response;
using CDD.Api.Models.Response.Admin.User;
using CDD.Api.Repositories;
using CDD.Api.Repositories.DTO;
using CDD.Api.Models.Shared;


namespace CDD.Api.Controllers
{
    [Route("api/Admin/User")]
    [ApiController]
    [LogOptions()]
    [ApiKeyAuthentication]
    [AdminTokenAuth]
    public class AdminUserController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        private readonly IConfiguration _config;

        private readonly ILogger _logger;

        private readonly IIPHelper _ipHelper;

        private readonly APIHelper _apiHelper;

        private readonly IMemoryCacheHelper _memoryCacheHelper;

        public readonly UserRepository _userRepository;

        private readonly ConfigurationSection _WebSetting;

        public AdminUserController(IWebHostEnvironment env, IConfiguration config, ILogger<AdminUserController> logger, APIHelper apiHelper,
            IIPHelper ipHelper,
            SystemRepository externalSystemRepo,
            Libs.IRequest request,
            IMemoryCacheHelper memoryCacheHelper,
            UserRepository userRepository)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiHelper = apiHelper ?? throw new ArgumentNullException(nameof(apiHelper));
            _ipHelper = ipHelper ?? throw new ArgumentNullException(nameof(ipHelper));
            _memoryCacheHelper = memoryCacheHelper ?? throw new ArgumentNullException(nameof(memoryCacheHelper));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _WebSetting = (ConfigurationSection)config.GetSection("WebSetting") ?? throw new ArgumentNullException("appsetting::WebSetting");
        }

        /// <summary>
        /// web 業務端 驗證User是否登入
        /// </summary>
        /// <returns></returns>
        [HttpPost("Login")]
        public CheckLoginResp Login([FromBody] LoginReq req)
        {
            CheckLoginResp resp = new CheckLoginResp { Status = false };
            SystemDTO systemInfo = Middlewares.ApiKeyAuthMiddleware.TryGetVerifiedSystemInfo(HttpContext) ?? throw MyExceptionList.UploadFileError($"Can not find systemID in HttpContext item ").GetException();

            string DecryptAccount = EncryptionHelper.DecryptString(req.Account, systemInfo.HashKey, systemInfo.IVKey);
            string DecryptPassword = EncryptionHelper.DecryptString(req.Password, systemInfo.HashKey, systemInfo.IVKey);
            User? userInfo = _userRepository.GetUserByUserID_AndCheckPassword(DecryptAccount, DecryptPassword);
            if (userInfo != null)
            {
                resp.Status = true;
                resp.Code = 0;
                resp.UserInfo = new UserSummary
                {
                    EmployeeNo = userInfo.EmployeeNo,
                    Email = userInfo.Email,
                    IsAdminUser = userInfo.IsAdminUser,
                    IsActive = userInfo.IsActive,
                    IsDeleted = userInfo.IsDeleted,
                    CreatedTime = userInfo.CreatedTime,
                    LastUpdate = userInfo.LastUpdate
                };
            }
            return resp;
        }

        /// <summary>
        /// 建立User
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("CreateUser")]
        public GeneralResp<object> CreateUser([FromBody] CreateUserReq req)
        {
            GeneralResp<object> resp = new GeneralResp<object>() { Status = true };
            // 1. 檢查 員編 是否存在
            User? existUserData = _userRepository.GetUserByUserID(req.EmployeeNo);
            if (existUserData != null)
            {
                throw MyExceptionList.UserAlreadyExists(req).GetException();
            }


            string pepper = String.Empty;
            string salt = String.Empty;
            string passwordHashed = HashHelper.GenHashedPassword(req.Password, out pepper, out salt, hashAlgorithm: HashAlgorithmName.SHA256);
            if (passwordHashed.Length != HashHelper.GetHashStringBase64Size(HashAlgorithmName.SHA256))
            {
                resp.Status = false;
                resp.Message = "GenHashedPassword error";
                return resp;
            }

            resp.Status = _userRepository.CreateUser(req.EmployeeNo, req.Email, passwordHashed);
            if (resp.Status)
            {
                resp.Code = 0;
            }

            return resp;
        }
    }
}
