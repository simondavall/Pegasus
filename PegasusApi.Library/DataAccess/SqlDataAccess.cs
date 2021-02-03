using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace PegasusApi.Library.DataAccess
{
    public class SqlDataAccess : IDataAccess
    {
        private readonly IConfiguration _configuration;

        public SqlDataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }

        public async Task<T> ExecuteScalarAsync<T, TParam>(string storedProcedure, TParam parameters, string connectionStringName)
        {
            var connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                return await connection.ExecuteScalarAsync<T>(storedProcedure, parameters, null, null, CommandType.StoredProcedure);
            }
        }

        public async Task<List<T>> LoadDataAsync<T, TParam>(string storedProcedure, TParam parameters, string connectionStringName)
        {
            var connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<T>(storedProcedure, parameters, null, null, CommandType.StoredProcedure);

                return rows.ToList();
            }
        }

        public async Task SaveDataAsync<TParam>(string storedProcedure, TParam parameters, string connectionStringName)
        {
            var connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(storedProcedure, parameters, null, null, CommandType.StoredProcedure);
            }
        }
    }
}
