using System.Linq;
using System.Threading.Tasks;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Library.DataAccess
{
    public class UsersData : IUsersData
    {
        private readonly IDataAccess _dataAccess;
        private const string ConnectionStringName = "Pegasus";

        public UsersData(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<UserModel> GetUser(string userId)
        {
            var output = await _dataAccess.LoadDataAsync<UserModel, dynamic>("spUsers_GetById", new { userId }, ConnectionStringName);
            return output.FirstOrDefault();
        }

        public async Task UpdateUser(UserModel user)
        {
            var parameters = new { user.Id, user.DisplayName };
            await _dataAccess.SaveDataAsync<dynamic>("spUsers_Update", parameters, ConnectionStringName);
        }
    }
}
