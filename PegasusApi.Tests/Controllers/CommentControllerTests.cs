using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models;

namespace PegasusApi.Tests.Controllers
{
    public class CommentControllerTests
    {
        private Mock<ICommentsData> _mockCommentsData;

        [SetUp]
        public void EachTestSetup()
        {
            _mockCommentsData = new Mock<ICommentsData>();
        }

        private PegasusApi.Controllers.CommentController CreateCommentController()
        {
            return new PegasusApi.Controllers.CommentController(_mockCommentsData.Object);
        }

        //TODO These tests demonstrate a lack of parameter validation. Need to add null checks etc.
        [Test]
        public async Task AddComment_CallsDbAddComment()
        {
            _mockCommentsData.Setup(x => x.AddComment(It.IsAny<TaskCommentModel>()));

            var sut = CreateCommentController();
            await sut.AddComment(new TaskCommentModel());

            _mockCommentsData.Verify(x => x.AddComment(It.IsAny<TaskCommentModel>()), Times.Once);
        }

        [Test]
        public async Task GetComments_CallsDbGetComments()
        {
            _mockCommentsData.Setup(x => x.GetComments(It.IsAny<int>()));

            var sut = CreateCommentController();
            await sut.GetComments(0);

            _mockCommentsData.Verify(x => x.GetComments(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task UpdateComments_CallsDbUpdateComments()
        {
            //TODO Add check for empty list (No need to call db if list empty
            _mockCommentsData.Setup(x => x.UpdateComments(It.IsAny<IEnumerable<TaskCommentModel>>()));

            var sut = CreateCommentController();
            await sut.UpdateComments(new List<TaskCommentModel>());

            _mockCommentsData.Verify(x => x.UpdateComments(It.IsAny<IEnumerable<TaskCommentModel>>()), Times.Once);
        }
    }
}