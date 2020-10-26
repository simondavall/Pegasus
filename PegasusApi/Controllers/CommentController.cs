using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PegasusApi.Models;

namespace PegasusApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        [Route("/getcomment")]
        [HttpGet]
        public TaskCommentModel GetComment(int id)
        {
            return new TaskCommentModel();

            //old code
            //    return _context.TaskComments.FirstOrDefault(c => c.Id == id);
        }

        [Route("/getcomments")]
        [HttpGet]
        public IEnumerable<TaskCommentModel> GetComments(int taskId)
        {
            return new []{ new TaskCommentModel() , new TaskCommentModel() } ;

            //old code
            //    return _context.TaskComments.Where(c => c.TaskId == taskId && !c.IsDeleted).OrderBy(c => c.Created);
        }

        [Route("/addcomment")]
        [HttpPost]
        public void AddComment(TaskCommentModel taskComment)
        {

            //old code
            //    _context.TaskComments.Add(taskComment);
            //    _context.SaveChanges();
        }

        [Route("/updatecomment")]
        [HttpPost]
        public void UpdateComment(TaskCommentModel taskComment)
        {

            //old code
            //    _context.TaskComments.Update(taskComment);
            //    _context.SaveChanges();
        }

        [Route("/updatecomments")]
        [HttpPost]
        public void UpdateComments(IEnumerable<TaskCommentModel> taskComments)
        {

            //old code
            //    foreach (var taskComment in taskComments)
            //    {
            //        _context.TaskComments.Update(taskComment);
            //    }
            //    _context.SaveChanges();
        }
    }
}
