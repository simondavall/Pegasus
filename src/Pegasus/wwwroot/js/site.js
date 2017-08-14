// Write your Javascript code.
$(function() {
    var currentSettings = {
        projectId : $("#projectId").val(),
        taskFilterId : $("#taskFilterId").val()
    }

    // disable the CreateIssues button when showing All Projects
    var createButton = $("#createButton");
    var setCreateIssueButton = function() {
        if (currentSettings.projectId === "0") {
            $(createButton).addClass("disabled");
        } else {
            $(createButton).removeClass("disabled");
        }
    }

    // set initial Create button state
    $(function () {
        setCreateIssueButton();
    });

    // hide all sidebar icons, then show the selected sidebar icon
    var projectListItems = $(".project-list-item");
    var taskFilterList = $(".task-filter");
    $(function () {
        $(projectListItems).find("i").addClass("hide");
        $(projectListItems[currentSettings.projectId]).find("i").removeClass("hide");
        $(taskFilterList).find("i").addClass("hide");
        $(taskFilterList[currentSettings.taskFilterId]).find("i").removeClass("hide");
    });

    var updateList = function () {
        var options = {
            url: $("form").attr("action"),
            type: $("form").attr("method"),
            data: $("form").serialize()
        };
        $.ajax(options).done(function (data) {
            var $target = $($("form").attr("data-pgs-target"));
            $target.replaceWith(data);
        });
    }

    var sidebarAction = function(item, hiddenItem, itemList) {
        var selectedId = $(item).attr("value");
        $(hiddenItem).val(selectedId);
        $(itemList).find("i").addClass("hide");
        $(item).find("i").removeClass("hide");
        updateList();
        return selectedId;
    };

    $(".body-content").on("click", ".project-list-item", function () {
        currentSettings.projectId = sidebarAction($(this), $("#projectId"), $(projectListItems));
        // need to disable Create button if All projects selected
        setCreateIssueButton();
        return false;
    });

    $(".body-content").on("click", ".task-filter", function () {
        sidebarAction($(this), $("#taskFilterId"), $(taskFilterList));
        return false;
    });

})