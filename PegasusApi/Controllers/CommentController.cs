using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models;

namespace PegasusApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentsData _commentsData;

        public CommentController(ICommentsData commentsData)
        {
            _commentsData = commentsData;
        }

        [Route("GetComments/{taskId}")]
        [HttpGet]
        public IEnumerable<TaskCommentModel> GetComments(int taskId)
        {
             return _commentsData.GetComments(taskId);
        }

        [Route("AddComment")]
        [HttpPost]
        public void AddComment(TaskCommentModel taskComment)
        {
            _commentsData.AddComment(taskComment);
        }


        [Route("UpdateComments")]
        [HttpPost]
        public void UpdateComments(IEnumerable<TaskCommentModel> taskComments)
        {
            _commentsData.UpdateComments(taskComments);
        }

        #region Not Yet Implemented

        [Obsolete]
        [Route("GetComment")]
        [HttpGet]
        public TaskCommentModel GetComment(int id)
        {
            return new TaskCommentModel();

            //old code
            //    return _context.TaskComments.FirstOrDefault(c => c.Id == id);
        }

        [Obsolete]
        [Route("UpdateComment")]
        [HttpPost]
        public void UpdateComment(TaskCommentModel taskComment)
        {

            //old code
            //    _context.TaskComments.Update(taskComment);
            //    _context.SaveChanges();
        }

        #endregion

    }
}
