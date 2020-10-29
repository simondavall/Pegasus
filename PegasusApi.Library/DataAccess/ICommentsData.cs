using System.Collections.Generic;
using PegasusApi.Library.Models;

namespace PegasusApi.Library.DataAccess
{
    public interface ICommentsData
    {
        List<TaskCommentModel> GetComments(int taskId);
        void AddComment(TaskCommentModel comment);
        void UpdateComment(TaskCommentModel comment);
        void UpdateComments(IEnumerable<TaskCommentModel> comments);
    }
}