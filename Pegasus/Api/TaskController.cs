using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Pegasus.Extensions;
using Pegasus.Library.Api;
using Pegasus.Library.Models;

namespace Pegasus.Api
{
    [Authorize(Roles = "PegasusUser")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITasksEndpoint _tasksEndpoint;

        public TaskController(ITasksEndpoint tasksEndpoint)
        {
            _tasksEndpoint = tasksEndpoint;
        }

        [HttpGet]
        public async Task<string> GetSubTasks(int taskId)
        {
            var result = await _tasksEndpoint.GetSubTasks(taskId);
            if (result is null)
            {
                return string.Empty;
            }

            foreach (var taskModel in result)
            {
                taskModel.TaskRefStyle = taskModel.TaskProfile().TaskRefStyle;
                taskModel.TaskNameStyle = taskModel.TaskProfile().TaskTextStyle;
                taskModel.TaskTimeStyle = taskModel.TaskProfile().TaskTimeStyle;
                taskModel.TaskIcon = taskModel.TaskProfile().TaskIcon;
                taskModel.LapsedTime = taskModel.Modified.LapsedTime();
            }

            return FormatToJson(result);
        }

        private static string FormatToJson(IEnumerable<TaskModel> subTasks)
        {
            var jArray = JArray.FromObject(subTasks,
                new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var jResult = new JObject
            {
                ["subTasks"] = jArray
            };

            return jResult.ToString(Formatting.None);
        }
    }
}
