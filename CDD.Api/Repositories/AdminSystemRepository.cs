using System.Data;
using Dapper;
using CDD.Api.Helpers;
using CDD.Api.Models.DTO;
using CDD.Api.Models.Request.Admin.System;

namespace CDD.Api.Repositories
{
    /// <summary>
    /// 系統別 AdminSystemRepository
    /// </summary>
    public class AdminSystemRepository : BaseRepository<AdminSystemRepository>
    {

        private readonly SystemRepository _systemRepo;
#nullable disable
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="dapperHelper"></param>
        /// <param name="systemRepo"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AdminSystemRepository(IConfiguration config, ILogger<AdminSystemRepository> logger, IDapperHelper dapperHelper, SystemRepository systemRepo) : base(config, logger, dapperHelper)
        {
            _systemRepo = systemRepo ?? throw new ArgumentNullException(nameof(systemRepo));
        }
#nullable restore

        /// <summary>
        /// 新增系統
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool CreateSystem(SystemCreateReq req)
        {
            Guid apiKey = (req.ApiKey != null) ? (Guid)req.ApiKey : Guid.NewGuid();
            string hashKey = (string.IsNullOrEmpty(req.HashKey) == false) ? req.HashKey : HashHelper.GenerateRandomString(32); // 32  Key AES-256
            string ivKey = (string.IsNullOrEmpty(req.IVKey) == false) ? req.IVKey : HashHelper.GenerateRandomString(16);
            string sql = _storedProcedures["usp_AdminSystemCreate"] ?? throw new ArgumentNullException("usp_AdminSystemCreate not found in appsetting");
            var param = new DynamicParameters();
            param.Add("system", req.SystemName);
            param.Add("ApiKey", apiKey);
            param.Add("HashKey", hashKey);
            param.Add("IVKey", ivKey);
            param.Add("IsActive", req.IsActive);
            int rowCount = _dapperHelper.Execute(ConnectionStringKey.CDD, sql, param, CommandType.StoredProcedure);
            return (rowCount == 1);
        }

        /// <summary>
        /// 停用/啟用系統
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public bool SystemDisableSwitching(string systemName, bool isActive)
        {
            DTO.SystemDTO _systemDTO = _systemRepo.GetSystemInfoBySystem(systemName) ?? throw new Exception($"無法找到 {systemName}");

            string sql = _storedProcedures["usp_AdminSystemDisableSwitching"] ?? throw new ArgumentNullException("usp_AdminSystemDisableSwitching not found in appsetting");
            DynamicParameters param = new DynamicParameters();
            param.Add("system", systemName);
            param.Add("IsActive", isActive);
            int rowCount = _dapperHelper.Execute(ConnectionStringKey.CDD, sql, param, CommandType.StoredProcedure);
            return (rowCount == 1);
        }

        /// <summary>
        /// 更新系統資訊
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public bool UpdateSystem(SystemUpdateReq req)
        {
            DTO.SystemDTO _systemDTO = _systemRepo.GetSystemInfoBySystem(req.SystemName) ?? throw new Exception($"無法找到 {req.SystemName}");
            Guid apiKey = (req.ApiKey != null) ? _systemDTO.ApiKey : Guid.NewGuid();
            string hashKey = (string.IsNullOrEmpty(req.HashKey) == false) ? req.HashKey : _systemDTO.HashKey; // 32  Key AES-256
            string ivKey = (string.IsNullOrEmpty(req.IVKey) == false) ? req.IVKey : _systemDTO.IVKey;

            string sql = _storedProcedures["usp_AdminSystemUpdate"] ?? throw new ArgumentNullException("usp_AdminSystemUpdate not found in appsetting");
            var param = new DynamicParameters();
            param.Add("System", req.SystemName);
            param.Add("ApiKey", apiKey);
            param.Add("HashKey", hashKey);
            param.Add("IVKey", ivKey);
            param.Add("IsActive", req.IsActive);
            int rowCount = _dapperHelper.Execute(ConnectionStringKey.CDD, sql, param, CommandType.StoredProcedure);
            return (rowCount == 1);
        }

        /// <summary>
        /// CDD 系統管理資料 後端分頁功能
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<DTO.Admin.SystemAdminDTO>? SystemViewData(string? system, int? pageNumber, int? pageSize)
        {
            string sql = _storedProcedures["usp_AdminSystemViewData"] ?? throw new ArgumentNullException("usp_AdminSystemViewData not found in appsetting");
            var param = new DynamicParameters();
            param.Add("System", string.IsNullOrEmpty(system) ? null : system);
            param.Add("PageNumber", pageNumber < 1 ? 1 : pageNumber);
            param.Add("PageSize", pageSize < 1 ? 10 : pageSize);
            return _dapperHelper.Query<DTO.Admin.SystemAdminDTO>(ConnectionStringKey.CDD, sql, param, CommandType.StoredProcedure);
        }

    }
}
