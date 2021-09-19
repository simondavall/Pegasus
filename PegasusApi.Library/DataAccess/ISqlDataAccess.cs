using System.Collections.Generic;
using System.Threading.Tasks;

namespace PegasusApi.Library.DataAccess
{
    public interface IDataAccess
    {
        Task<T> ExecuteScalarAsync<T, TParam>(string storedProcedure, TParam parameters, string connectionStringName);
        Task<List<T>> LoadDataAsync<T, TParam>(string storedProcedure, TParam parameters, string connectionStringName);
        Task SaveDataAsync<TParam>(string storedProcedure, TParam parameters, string connectionStringName);
    }
}