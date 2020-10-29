using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public List<T> LoadData<T, TParam>(string storedProcedure, TParam parameters, string connectionStringName)
        {
            var connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var rows = connection.Query<T>(storedProcedure, parameters, null, true, null, CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public void SaveData<TParam>(string storedProcedure, TParam parameters, string connectionStringName)
        {
            var connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters, null, null, CommandType.StoredProcedure);
            }
        }
    }
}
