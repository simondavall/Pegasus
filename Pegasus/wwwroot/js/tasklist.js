$(function () {

    var getSubTasks = function (taskId, callback) {
        $.ajax({
            url: `/api/Task/GetSubTasks?taskId=${taskId}`,
            complete:
                function (json) {
                    if (json.status === 200) {
                        callback(JSON.parse(json.responseText));
                    } else {
                        console.log(`Failed to load /getSubTasks?${taskId}`, json.status, json.statusText);
                    }
                }
        });
    }

    var updateView = function (target, subTasks) {
        var subTasksStr = "";

        for (var i in subTasks) {
            subTasksStr += "<li>";
            subTasksStr += `<span>${subTasks[i].taskRef}</span>`;
            subTasksStr += `<span>${subTasks[i].name}</span>`;
            subTasksStr += "<span>LapsedTime</span>";
            subTasksStr += "</li>";
        }
        target.append(subTasksStr);
    }
    
    $(".body-content").on("click", ".task-list-subtask-link", function () {
        var taskId = $(this).attr("data-route-id");
        var targetId = $(this).attr("data-subtask-target");
        var target = $(`.${targetId}`);
        if (target.children("li").length) {
            target.empty();
            return;
        }
        getSubTasks(taskId, function (json) {
            if (json.hasOwnProperty("subTasks")) {
                updateView(target, json.subTasks);
            } else {
                if (json.hasOwnProperty("status")) {
                    console.log("Failed request to api/task/getSubTasks?", json.status);
                } else {
                    console.log("Bad response obj from getSubTasks");
                }
            }
        });
    });

})