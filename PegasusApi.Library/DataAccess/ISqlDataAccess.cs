using System.Collections.Generic;
using System.Threading.Tasks;

namespace PegasusApi.Library.DataAccess
{
    public interface IDataAccess
    {
        Task<List<T>> LoadDataAsync<T, TParam>(string storedProcedure, TParam parameters, string connectionStringName);
        Task SaveDataAsync<TParam>(string storedProcedure, TParam parameters, string connectionStringName);
    }
}