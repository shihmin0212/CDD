using System.Data;
using Dapper;
using CDD.Api.Helpers;
using CDD.Api.Models.DTO;

namespace CDD.Api.Repositories
{
    /// <summary>
    /// 系統別 SystemRepository
    /// </summary>
    public class SystemRepository : BaseRepository<SystemRepository>
    {
#nullable disable
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="dapperHelper"></param>
        public SystemRepository(IConfiguration config, ILogger<SystemRepository> logger, IDapperHelper dapperHelper) : base(config, logger, dapperHelper)
        { }
#nullable restore


        /// <summary>
        /// 取得 業務端外部系統 資訊(Api Key)
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public DTO.SystemDTO? GetSystemByApiKey(Guid apiKey)
        {
            string sql = _storedProcedures["usp_GetSystemByApiKey"] ?? throw new ArgumentNullException("usp_GetSystemByApiKey not found in appsetting");
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ApiKey", apiKey);
            IEnumerable<DTO.SystemDTO>? rows;
            rows = _dapperHelper.Query<DTO.SystemDTO>(ConnectionStringKey.CDD, sql, param: parameters, CommandType.StoredProcedure);
            return rows?.FirstOrDefault();
        }


        /// <summary>
        /// 取得 業務端外部系統 資訊(System)
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        public DTO.SystemDTO? GetSystemInfoBySystem(string systemID)
        {
            string sql = _storedProcedures["usp_GetSystemInfoBySystem"] ?? throw new ArgumentNullException("usp_GetSystemInfoBySystem not found in appsetting");
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("System", systemID);
            IEnumerable<DTO.SystemDTO>? rows;
            rows = _dapperHelper.Query<DTO.SystemDTO>(ConnectionStringKey.CDD, sql, param: parameters, CommandType.StoredProcedure);
            return rows?.FirstOrDefault();
        }

        /// <summary>
        /// 取得所有 業務端外部系統 資訊
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<DTO.SystemDTO>? GetSystemList()
        {
            string sql = _storedProcedures["usp_GetSystemList"] ?? throw new ArgumentNullException("usp_GetExternalSystemList not found in appsetting");
            return _dapperHelper.Query<DTO.SystemDTO>(ConnectionStringKey.CDD, sql, CommandType.StoredProcedure);
        }



    }
}
