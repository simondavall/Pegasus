using System.Collections.Generic;
using System.Threading.Tasks;
using PegasusApi.Library.Models;

namespace PegasusApi.Library.DataAccess
{
    public class CommentsData : ICommentsData
    {
        private readonly IDataAccess _dataAccess;
        private const string ConnectionStringName = "Pegasus";

        public CommentsData(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<List<TaskCommentModel>> GetComments(int taskId)
        {
            var output = await _dataAccess.LoadDataAsync<TaskCommentModel, dynamic>("spComments_GetAllForTask", new { taskId }, ConnectionStringName);
            return output;
        }

        public async Task AddComment(TaskCommentModel comment)
        {
            var parameters = new { comment.TaskId, comment.Comment, comment.UserId };
            await _dataAccess.SaveDataAsync<dynamic>("spComments_Add", parameters, ConnectionStringName);
        }

        public async Task UpdateComment(TaskCommentModel comment)
        {
            var parameters = new { comment.Id, comment.Comment, comment.IsDeleted };
            await _dataAccess.SaveDataAsync<dynamic>("spComments_Update", parameters, ConnectionStringName);
        }

        public async Task UpdateComments(IEnumerable<TaskCommentModel> comments)
        {
            foreach (var comment in comments)
            {
                await UpdateComment(comment);
            }
        }

    }
}
