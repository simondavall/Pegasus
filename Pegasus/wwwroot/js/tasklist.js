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

    var formatTaskRef = function (taskRef, taskRefStyle) {
        var str = "";
        str += `<div class=\"sub-task-list-ref ${taskRefStyle}\" >`;
        str += taskRef;
        str += "</div>";
        return str;
    }

    var formatTaskName = function (taskName, taskNameStyle, taskIcon) {
        var str = "";
        str += `<div class=\"sub-task-list-name ${taskNameStyle}\" >`;
        str += taskIcon;
        str += taskName;
        str += "</div>";
        return str;
    }

    var formatTime = function (taskTime, taskTimeStyle) {
        var str = "";
        str += `<div class=\"sub-task-list-time ${taskTimeStyle}\" >(`;
        str += taskTime;
        str += ")</div>";
        return str;
    }

    var updateView = function (target, subTasks) {
        var subTasksStr = "";

        for (var i in subTasks) {
            subTasksStr += "<li>";
            subTasksStr += `<a href=\"/TaskList/Edit/${subTasks[i].id}\" class=\"sub-task-list-row\">`;
            subTasksStr += formatTaskRef(subTasks[i].taskRef, subTasks[i].taskRefStyle);
            subTasksStr += formatTaskName(subTasks[i].name, subTasks[i].taskNameStyle, subTasks[i].taskIcon);
            subTasksStr += formatTime("2 days ago", subTasks[i].taskNameStyle);
            subTasksStr += "</a>";
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