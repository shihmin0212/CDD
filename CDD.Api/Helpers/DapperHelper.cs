using System.Data;
using System.Data.Common;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Sample.Api.Helpers
{
    public interface IDapperHelper
    {
        IDbConnection CreateConnection(string connectionKey);

        IEnumerable<T>? Query<T>(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure);

        Task<IEnumerable<T>?> QueryAsync<T>(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure);

        int Execute(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure);

        int Execute(IDbConnection connection, IDbTransaction transaction, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure);

        Task<int> ExecuteAsync(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure);

        Task<int> ExecuteAsync(IDbConnection connection, IDbTransaction transaction, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure);

        DataTable? ExecuteReader(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure);

        Task<DataTable?> ExecuteReaderAsync(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure);
    }

    public class DapperHelper : IDapperHelper
    {
        private readonly IConfiguration _config;

        public DapperHelper(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateConnection(string connectionKey)
        {
            string conn = _config.GetConnectionString(connectionKey) ?? throw new ArgumentNullException($"Get Connection String Fail:{connectionKey}");
            return new SqlConnection(conn);
        }


        #region Query 
        public IEnumerable<T>? Query<T>(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection connection = CreateConnection(connectionKey))
            {
                return connection.Query<T>(spName, param, commandType: commandType);
            }
        }

        public async Task<IEnumerable<T>?> QueryAsync<T>(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using (IDbConnection connection = CreateConnection(connectionKey))
            {
                return await connection.QueryAsync<T>(spName, param, commandType: commandType);
            }
        }
        #endregion



        #region Execute Transaction 以交易單筆執行sql
        /// <summary>
        /// 執行
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <param name="spName"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public int Execute(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            int rowCount = 0;
            using (IDbConnection connection = CreateConnection(connectionKey))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    rowCount = connection.QuerySingle<int>(spName, param, transaction: transaction, commandType: commandType);
                    // 如果所有操作成功，提交交易
                    transaction.Commit();
                    connection.Close(); // 關閉連線
                    return rowCount;
                }
            }
        }


        /// <summary>
        /// 執行
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <param name="spName"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            int rowCount = 0;
            using (IDbConnection connection = CreateConnection(connectionKey))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    rowCount = await connection.QuerySingleAsync<int>(spName, param, transaction: transaction, commandType: commandType);
                    // 如果所有操作成功，提交交易
                    transaction.Commit();
                    connection.Close(); // 關閉連線
                    return rowCount;
                }
            }
        }


        #endregion


        #region Execute Transaction 傳入transaction 以多筆執行sql
        /// <summary>
        /// 執行
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="spName"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public int Execute(IDbConnection connection, IDbTransaction transaction, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            int rowCount = 0;
            rowCount = connection.QuerySingle<int>(spName, param, transaction, commandType: commandType);
            return rowCount;
        }


        /// <summary>
        /// 執行
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="spName"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(IDbConnection connection, IDbTransaction transaction, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            int rowCount = 0;
            rowCount = await connection.QuerySingleAsync<int>(spName, param, transaction, commandType: commandType);
            return rowCount;
        }



        #endregion

        #region Return Data As DataTable
        public DataTable? ExecuteReader(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            DataTable? table = null;
            using (IDbConnection connection = CreateConnection(connectionKey))
            {
                IDataReader reader = connection.ExecuteReader(spName, param, commandType: commandType);
                if (((DbDataReader)reader).HasRows)
                {
                    table = new DataTable();
                    table.Load(reader);
                }
            }
            return table;
        }

        public async Task<DataTable?> ExecuteReaderAsync(string connectionKey, string spName, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            DataTable? table = null;
            using (IDbConnection connection = CreateConnection(connectionKey))
            {
                IDataReader reader = await connection.ExecuteReaderAsync(spName, param, commandType: commandType);
                if (((DbDataReader)reader).HasRows)
                {
                    table = new DataTable();
                    table.Load(reader);
                }
            }
            return table;
        }
        #endregion
    }

}
