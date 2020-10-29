using System.Collections.Generic;
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

        public List<TaskCommentModel> GetComments(int taskId)
        {
            var output = _dataAccess.LoadData<TaskCommentModel, dynamic>("spComments_GetAllForTask", new { taskId }, ConnectionStringName);
            return output;
        }

        public void AddComment(TaskCommentModel comment)
        {
            var parameters = new { comment.TaskId, comment.Comment };
            _dataAccess.SaveData<dynamic>("spComments_Add", parameters, ConnectionStringName);
        }

        public void UpdateComment(TaskCommentModel comment)
        {
            var parameters = new { comment.Id, comment.Comment, comment.IsDeleted };
            _dataAccess.SaveData<dynamic>("spComments_Update", parameters, ConnectionStringName);
        }

        public void UpdateComments(IEnumerable<TaskCommentModel> comments)
        {
            foreach (var comment in comments)
            {
                UpdateComment(comment);
            }
        }

    }
}
