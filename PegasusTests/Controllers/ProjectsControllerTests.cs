using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Api;
using Pegasus.Library.Models;

namespace PegasusTests.Controllers
{
    class ProjectsControllerTests
    {
        private Mock<IProjectsEndpoint> _mockProjectsEndpoint;

        [SetUp]
        public void TestSetup()
        {
            _mockProjectsEndpoint = new Mock<IProjectsEndpoint>();
        }

        [Test]
        public void GET_Create_ReturnsViewResult()
        {
            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = sut.Create();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task POST_Create_InvalidModel_ReturnsViewResult()
        {
            var projectsEndpoint = new Mock<IProjectsEndpoint>();

            var sut = new ProjectsController(projectsEndpoint.Object);
            sut.ModelState.AddModelError("Code.", "errorMessage");
            var result = await sut.Create(new ProjectModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ProjectModel>(((ViewResult)result).Model);
        }

        [Test]
        public async Task POST_Create_ValidModel_ReturnsRedirectToAction()
        {
            _mockProjectsEndpoint.Setup(x => x.AddProject(It.IsAny<ProjectModel>()));

            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Create(new ProjectModel());


            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(ProjectsController.Index), ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task GET_Delete_IdIsNull_ReturnsNotFound()
        {
            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Delete(null);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GET_Delete_ProjectIsNull_ReturnsNotFound()
        {
            _mockProjectsEndpoint.Setup(x => x.GetProject(It.IsAny<int>())).ReturnsAsync((ProjectModel)null);

            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Delete(123);

            _mockProjectsEndpoint.Verify(x => x.GetProject(It.IsAny<int>()), Times.Exactly(1));
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GET_Delete_ProjectNotNull_ReturnsViewResult()
        {
            _mockProjectsEndpoint.Setup(x => x.GetProject(It.IsAny<int>())).ReturnsAsync(new ProjectModel());

            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Delete(123);

            _mockProjectsEndpoint.Verify(x => x.GetProject(It.IsAny<int>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ProjectModel>(((ViewResult)result).Model);
        }

        [Test]
        public async Task POST_Delete_ReturnsRedirectToAction()
        {
            _mockProjectsEndpoint.Setup(x => x.DeleteProject(It.IsAny<int>()));

            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.DeleteConfirmed(123);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(ProjectsController.Index), ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task GET_Edit_IdIsNull_ReturnsNotFound()
        {
            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Edit(null);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GET_Edit_ProjectIsNull_ReturnsNotFound()
        {
            _mockProjectsEndpoint.Setup(x => x.GetProject(It.IsAny<int>())).ReturnsAsync((ProjectModel)null);

            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Edit(123);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GET_Edit_ProjectNotNull_ReturnsViewResult()
        {
            _mockProjectsEndpoint.Setup(x => x.GetProject(It.IsAny<int>())).ReturnsAsync(new ProjectModel());

            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Edit(123);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ProjectModel>(((ViewResult)result).Model);
        }

        [Test]
        public async Task POST_Edit_MismatchedProjectIds_ReturnsNotFound()
        {
            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Edit(123, new ProjectModel { Id = 321 });

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task POST_Edit_ModelInvalid_ReturnsViewResult()
        {
            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            sut.ModelState.AddModelError("Code", "Error");
            var result = await sut.Edit(123, new ProjectModel { Id = 123 });

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ProjectModel>(((ViewResult)result).Model);
        }

        [Test]
        public async Task POST_Edit_ExceptionProjectNotFound_ReturnsNotFound()
        {
            _mockProjectsEndpoint.Setup(x => x.UpdateProject(It.IsAny<ProjectModel>())).ThrowsAsync(new Exception());
            _mockProjectsEndpoint.Setup(x => x.GetProject(It.IsAny<int>())).ReturnsAsync(new ProjectModel());

            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Edit(123, new ProjectModel { Id = 123 });

            _mockProjectsEndpoint.Verify(x => x.UpdateProject(It.IsAny<ProjectModel>()), Times.Exactly(1));
            _mockProjectsEndpoint.Verify(x => x.GetProject(It.IsAny<int>()), Times.Exactly(1));
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task POST_Edit_ExceptionProjectFound_ReturnsViewResultWithErrorMessage()
        {
            _mockProjectsEndpoint.Setup(x => x.UpdateProject(It.IsAny<ProjectModel>())).ThrowsAsync(new Exception());
            _mockProjectsEndpoint.Setup(x => x.GetProject(It.IsAny<int>())).ReturnsAsync(new ProjectModel { Id = 123 });

            var sut = new ProjectsController(_mockProjectsEndpoint.Object);

            var result = await sut.Edit(123, new ProjectModel { Id = 123 });

            _mockProjectsEndpoint.Verify(x => x.UpdateProject(It.IsAny<ProjectModel>()), Times.Exactly(1));
            _mockProjectsEndpoint.Verify(x => x.GetProject(It.IsAny<int>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ProjectModel>(((ViewResult)result).Model);
        }

        [Test]
        public async Task POST_Edit_ProjectSaved_ReturnRedirectToAction()
        {
            _mockProjectsEndpoint.Setup(x => x.UpdateProject(It.IsAny<ProjectModel>()));

            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Edit(123, new ProjectModel { Id = 123 });

            _mockProjectsEndpoint.Verify(x => x.UpdateProject(It.IsAny<ProjectModel>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(ProjectsController.Index), ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task GET_Index_ReturnViewResult()
        {
            var list = new List<ProjectModel>
            {
                new ProjectModel {Id = 123, Name= "B", IsActive = true, IsPinned = true, ProjectPrefix = "TST"},
                new ProjectModel {Id = 111, Name= "A", IsActive = true, IsPinned = false, ProjectPrefix = "TST"}
            };

            _mockProjectsEndpoint.Setup(x => x.GetAllProjects()).ReturnsAsync(list);

            var sut = new ProjectsController(_mockProjectsEndpoint.Object);
            var result = await sut.Index();

            _mockProjectsEndpoint.Verify(x => x.GetAllProjects(), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<IOrderedEnumerable<ProjectModel>>(((ViewResult)result).Model);

            var returnedList = ((IOrderedEnumerable<ProjectModel>)((ViewResult)result).Model).ToList();
            Assert.AreNotEqual(list[0].Name, returnedList[0].Name);
            Assert.AreEqual(111, returnedList[0].Id);
            Assert.AreEqual(true, returnedList[0].IsActive);
            Assert.AreEqual(false, returnedList[0].IsPinned);
            Assert.AreEqual("TST", returnedList[0].ProjectPrefix);
        }
    }
}
