// Write your Javascript code.
$(function () {
    var currentProjectId = $("#projectId").val();
    var createButton = $("#createButton");
    $(function () {
        if (currentProjectId === "0") {
            $(createButton).addClass("disabled");
        } else {
            $(createButton).removeClass("disabled");
        }
    });

    var projectListItems = $(".project-list-item");
    $(function () {
        $(projectListItems).find("i").addClass("hide");
        $(projectListItems[currentProjectId]).find("i").removeClass("hide");
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

    $(".body-content").on("click", ".project-list-item", function () {
        var newProjectId = $(this).attr("value");
        $("#projectId").val(newProjectId);
        $(projectListItems).find("i").addClass("hide");
        updateList();
        $(this).find("i").removeClass("hide");
        if (newProjectId === "0") {
            $(createButton).addClass("disabled");
        } else {
            $(createButton).removeClass("disabled");
        }
        return false;
    });

})