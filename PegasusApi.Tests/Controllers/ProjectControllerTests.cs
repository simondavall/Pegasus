using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models;

namespace PegasusApi.Tests.Controllers
{
    public class ProjectControllerTests
    {
        private Mock<IProjectsData> _mockProjectsData;

        [SetUp]
        public void EachTestSetup()
        {
            _mockProjectsData = new Mock<IProjectsData>();
        }

        private PegasusApi.Controllers.ProjectController CreateProjectController()
        {
            return new PegasusApi.Controllers.ProjectController(_mockProjectsData.Object);
        }

        //TODO These tests demonstrate a lack of parameter validation. Need to add null checks etc.
        [Test]
        public async Task AddProject_CallsDbAddProject()
        {
            _mockProjectsData.Setup(x => x.AddProject(It.IsAny<ProjectModel>()));

            var sut = CreateProjectController();
            await sut.AddProject(new ProjectModel());

            _mockProjectsData.Verify(x => x.AddProject(It.IsAny<ProjectModel>()), Times.Once);
        }
        
        [Test]
        public async Task DeleteProject_CallsDbDeleteProject()
        {
            var projectId = 0;
            _mockProjectsData.Setup(x => x.DeleteProject(It.IsAny<int>()));

            var sut = CreateProjectController();
            await sut.DeleteProject(projectId);

            _mockProjectsData.Verify(x => x.DeleteProject(It.IsAny<int>()), Times.Once);
        }
        
        [Test]
        public async Task GetAllProjects_CallsDbGetProjects()
        {
            _mockProjectsData.Setup(x => x.GetProjects());

            var sut = CreateProjectController();
            await sut.GetAllProjects();

            _mockProjectsData.Verify(x => x.GetProjects(), Times.Once);
        }
        
        [Test]
        public async Task GetProject_CallsDbGetProject()
        {
            var projectId = 0;
            _mockProjectsData.Setup(x => x.GetProject(It.IsAny<int>()));

            var sut = CreateProjectController();
            await sut.GetProject(projectId);

            _mockProjectsData.Verify(x => x.GetProject(It.IsAny<int>()), Times.Once);
        }
        
        [Test]
        public async Task UpdateProject_CallsDbUpdateProject()
        {
            _mockProjectsData.Setup(x => x.UpdateProject(It.IsAny<ProjectModel>()));

            var sut = CreateProjectController();
            await sut.UpdateProject(new ProjectModel());

            _mockProjectsData.Verify(x => x.UpdateProject(It.IsAny<ProjectModel>()), Times.Once);
        }
    }
}