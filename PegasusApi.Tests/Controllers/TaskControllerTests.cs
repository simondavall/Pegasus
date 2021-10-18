using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models;

namespace PegasusApi.Tests.Controllers
{
    public class TaskControllerTests
    {
        private Mock<ITasksData> _mockTaskData;

        [SetUp]
        public void EachTestSetup()
        {
            _mockTaskData = new Mock<ITasksData>();
        }

        private PegasusApi.Controllers.TaskController CreateTaskController()
        {
            return new PegasusApi.Controllers.TaskController(_mockTaskData.Object);
        }

        //TODO These tests demonstrate a lack of parameter validation. Need to add null checks etc.
        [Test]
        public async Task AddTask_CallsDbAddTask()
        {
            _mockTaskData.Setup(x => x.AddTask(It.IsAny<TaskModel>()));

            var sut = CreateTaskController();
            await sut.AddTask(new TaskModel());
    
            _mockTaskData.Verify(x => x.AddTask(It.IsAny<TaskModel>()), Times.Once);
        }
        
        [Test]
        public async Task AddTaskStatus_CallsDbAddTaskStatus()
        {
            _mockTaskData.Setup(x => x.AddTaskStatus(It.IsAny<TaskStatusModel>()));

            var sut = CreateTaskController();
            await sut.AddTaskStatus(new TaskStatusModel());
    
            _mockTaskData.Verify(x => x.AddTaskStatus(It.IsAny<TaskStatusModel>()), Times.Once);
        }
        
        [Test]
        public async Task AddTaskType_CallsDbAddTaskType()
        {
            _mockTaskData.Setup(x => x.AddTaskType(It.IsAny<TaskTypeModel>()));

            var sut = CreateTaskController();
            await sut.AddTaskType(new TaskTypeModel());
    
            _mockTaskData.Verify(x => x.AddTaskType(It.IsAny<TaskTypeModel>()), Times.Once);
        }
        
        [Test]
        public async Task AddTaskPriority_CallsDbAddTaskPriority()
        {
            _mockTaskData.Setup(x => x.AddTaskPriority(It.IsAny<TaskPriorityModel>()));

            var sut = CreateTaskController();
            await sut.AddTaskPriority(new TaskPriorityModel());
    
            _mockTaskData.Verify(x => x.AddTaskPriority(It.IsAny<TaskPriorityModel>()), Times.Once);
        }
        
        [Test]
        public async Task GetAllTAsks_CallsDbGetAllTasks()
        {
            _mockTaskData.Setup(x => x.GetAllTasks());

            var sut = CreateTaskController();
            await sut.GetAllTasks();
    
            _mockTaskData.Verify(x => x.GetAllTasks(), Times.Once);
        }
        
        [Test]
        public async Task GetSubTasks_CallsDbGetSubTasks()
        {
            var parentTaskId = 0;
            _mockTaskData.Setup(x => x.GetSubTasks(It.IsAny<int>()));

            var sut = CreateTaskController();
            await sut.GetSubTasks(parentTaskId);
    
            _mockTaskData.Verify(x => x.GetSubTasks(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task GetAllTaskPriorities_CallsDbGetAllTaskPriorities()
        {
            _mockTaskData.Setup(x => x.GetAllTaskPriorities());

            var sut = CreateTaskController();
            await sut.GetAllTaskPriorities();
    
            _mockTaskData.Verify(x => x.GetAllTaskPriorities(), Times.Once);
        }
        
        [Test]
        public async Task GetAllTaskStatuses_CallsDbGetAllTaskStatuses()
        {
            _mockTaskData.Setup(x => x.GetAllTaskStatuses());

            var sut = CreateTaskController();
            await sut.GetAllTaskStatuses();
    
            _mockTaskData.Verify(x => x.GetAllTaskStatuses(), Times.Once);
        }
        
        [Test]
        public async Task GetAllTaskTypes_CallsDbGetAllTaskTypes()
        {
            _mockTaskData.Setup(x => x.GetAllTaskTypes());

            var sut = CreateTaskController();
            await sut.GetAllTaskTypes();
    
            _mockTaskData.Verify(x => x.GetAllTaskTypes(), Times.Once);
        }
        
        [Test]
        public async Task GetStatusHistory_CallsDbGetStatusHistory()
        {
            var taskId = 0;
            _mockTaskData.Setup(x => x.GetStatusHistory(It.IsAny<int>()));

            var sut = CreateTaskController();
            await sut.GetStatusHistory(taskId);
    
            _mockTaskData.Verify(x => x.GetStatusHistory(It.IsAny<int>()), Times.Once);
        }
        
        [Test]
        public async Task GetTask_CallsDbGetTask()
        {
            var taskId = 0;
            _mockTaskData.Setup(x => x.GetTask(It.IsAny<int>()));

            var sut = CreateTaskController();
            await sut.GetTask(taskId);
    
            _mockTaskData.Verify(x => x.GetTask(It.IsAny<int>()), Times.Once);
        }
        
        [Test]
        public async Task GetTasks_CallsDbGetTasks()
        {
            var projectId = 0;
            _mockTaskData.Setup(x => x.GetTasks(It.IsAny<int>()));

            var sut = CreateTaskController();
            await sut.GetTasks(projectId);
    
            _mockTaskData.Verify(x => x.GetTasks(It.IsAny<int>()), Times.Once);
        }
        
        [Test]
        public async Task UpdateTask_CallsDbUpdateTask()
        {
            _mockTaskData.Setup(x => x.UpdateTask(It.IsAny<TaskModel>()));

            var sut = CreateTaskController();
            await sut.UpdateTask(new TaskModel());
    
            _mockTaskData.Verify(x => x.UpdateTask(It.IsAny<TaskModel>()), Times.Once);
        }
    }
}