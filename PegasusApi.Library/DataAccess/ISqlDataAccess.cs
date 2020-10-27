using System.Collections.Generic;

namespace PegasusApi.Library.DataAccess
{
    public interface IDataAccess
    {
        List<T> LoadData<T, TParam>(string storedProcedure, TParam parameters, string connectionStringName);
        void SaveData<TParam>(string storedProcedure, TParam parameters, string connectionStringName);
    }
}