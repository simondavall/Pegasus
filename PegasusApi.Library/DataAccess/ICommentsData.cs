using System.Collections.Generic;
using System.Threading.Tasks;
using PegasusApi.Library.Models;

namespace PegasusApi.Library.DataAccess
{
    public interface ICommentsData
    {
        Task<List<TaskCommentModel>> GetComments(int taskId);
        Task AddComment(TaskCommentModel comment);
        Task UpdateComment(TaskCommentModel comment);
        Task UpdateComments(IEnumerable<TaskCommentModel> comments);
    }
}