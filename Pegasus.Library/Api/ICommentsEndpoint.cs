using System.Collections.Generic;
using System.Threading.Tasks;
using Pegasus.Library.Models;

namespace Pegasus.Library.Api
{
    public interface ICommentsEndpoint
    {
        Task AddComment(TaskCommentModel taskComment);
        Task<List<TaskCommentModel>> GetComments(int taskId);
        Task UpdateComments(IEnumerable<TaskCommentModel> taskComments);
    }
}