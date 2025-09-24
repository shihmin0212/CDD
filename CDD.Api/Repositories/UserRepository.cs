using System.Data;
using System.Security.Cryptography;
using Dapper;
using CDD.Api.Helpers;
using CDD.Api.Models.DTO;
using CDD.Api.Repositories.DTO;
using CDD.Api.Repositories.Shared;

namespace CDD.Api.Repositories
{
    public class UserRepository : BaseRepository<UserRepository>
    {
#nullable disable
        public UserRepository(IConfiguration config, ILogger<UserRepository> logger, IDapperHelper dapperHelper) : base(config, logger, dapperHelper)
        { }
#nullable restore

        /// <summary>
        /// 取得所有用戶
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User>? GetAllUser()
        {
            string sql = _storedProcedures["usp_GetAllUser"] ?? throw new ArgumentNullException("usp_GetAllUser not found in appsetting");
            return _dapperHelper.Query<User>(ConnectionStringKey.CDD, sql, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 取得員工資料by員編
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public User? GetUserByUserID(string userID)
        {
            // bind From Appsetting
            List<Repositories.DTO.User> AdminUsers = new List<Repositories.DTO.User> { };
            _config.GetSection("AdminUsers")?.Bind(AdminUsers);

            User? AdminUserFromAppsetting = AdminUsers.Where(x => x.EmployeeNo.Equals(userID)).FirstOrDefault();
            if (AdminUserFromAppsetting != null)
            {
                return AdminUserFromAppsetting;
            }

            string sql = _storedProcedures["usp_GetUserByEmployeeNo"] ?? throw new ArgumentNullException("usp_GetUserByEmployeeNo not found in appsetting");
            var param = new DynamicParameters();
            param.Add("EmployeeNo", userID);
            IEnumerable<User>? rows = _dapperHelper.Query<User>(ConnectionStringKey.CDD, sql, param, CommandType.StoredProcedure);
            return rows?.SingleOrDefault();
        }

        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="plainTextPassword"></param>
        /// <returns></returns>
        public User? GetUserByUserID_AndCheckPassword(string userNo, string plainTextPassword)
        {
            User? user = GetUserByUserID(userNo);
            if (user != null)
            {
                string base64Password = HashHelper.GenHashedPassword(plainTextPassword, user.Pepper, user.Base64Salt, hashAlgorithm: HashAlgorithmName.SHA256);
                if (user.Base64PasswordSignature.Equals(base64Password))
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// 建立User
        /// </summary>
        /// <param name="employeeNo"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool CreateUser(string employeeNo, string email, string password)
        {
            string pepper;
            string base64Salt;
            string hashedPassword = HashHelper.GenHashedPassword(password, out pepper, out base64Salt, hashAlgorithm: HashAlgorithmName.SHA256);
            string sql = _storedProcedures["usp_CreateUser"] ?? throw new ArgumentNullException("usp_CreateUser not found in appsetting");
            var param = new DynamicParameters();
            param.Add("EmployeeNo", employeeNo);
            param.Add("Email", email);
            param.Add("IsActive", true);
            param.Add("IsAdminUser", false);
            param.Add("IsDeleted", false);
            param.Add("Base64PasswordSignature", hashedPassword);
            param.Add("Base64Salt", base64Salt);
            param.Add("Pepper", pepper);
            int rowCount = _dapperHelper.Execute(ConnectionStringKey.CDD, sql, param, CommandType.StoredProcedure);
            return (rowCount == 1);
        }
    }
}
