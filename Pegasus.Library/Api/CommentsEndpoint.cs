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

    public class CommentsEndpoint : ICommentsEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public CommentsEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task AddComment(TaskCommentModel taskComment)
        {
            await _apiHelper.PostAsync(taskComment, "api/Comment/AddComment");
        }

        public async Task<List<TaskCommentModel>> GetComments(int taskId)
        {
            return await _apiHelper.GetListFromUri<TaskCommentModel>($"api/Comment/GetComments/{taskId}");
        }

        public async Task UpdateComments(IEnumerable<TaskCommentModel> taskComments)
        {
            await _apiHelper.PostAsync(taskComments, "api/Comment/UpdateComments");
        }
    }
}
