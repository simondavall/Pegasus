using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models;

namespace PegasusApi.Controllers
{
    [Authorize(Roles = "PegasusUser")]
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
        public async Task<IEnumerable<TaskCommentModel>> GetComments(int taskId)
        {
             return await _commentsData.GetComments(taskId);
        }

        [Route("AddComment")]
        [HttpPost]
        public async Task AddComment(TaskCommentModel taskComment)
        {
            await _commentsData.AddComment(taskComment);
        }

        [Route("UpdateComments")]
        [HttpPost]
        public async Task UpdateComments(IEnumerable<TaskCommentModel> taskComments)
        {
            await _commentsData.UpdateComments(taskComments);
        }
    }
}
