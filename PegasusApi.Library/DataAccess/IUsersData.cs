using System.Threading.Tasks;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Library.DataAccess
{
    public interface IUsersData
    {
        Task<UserModel> GetUser(string userId);
        Task UpdateUser(UserModel user);
    }
}
