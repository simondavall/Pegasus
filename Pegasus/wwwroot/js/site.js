// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// ReSharper disable UseOfImplicitGlobalInFunctionScope
// ReSharper disable PossiblyUnassignedProperty

// Write your JavaScript code.
$(function () {
    var currentSettings = {
        projectId: $("#projectId").val(),
        taskFilterId: $("#taskFilterId").val()
    }

    // disable the CreateTask button when showing All Projects
    var createButton = $("#createButton");
    var setCreateTaskButton = function () {
        if (currentSettings.projectId === "0") {
            $(createButton).addClass("disabled");
        } else {
            $(createButton).removeClass("disabled");
        }
    }

    // set initial Create button state
    $(function () {
        setCreateTaskButton();
    });

    // hide all sidebar icons, then show the selected sidebar icon
    var projectListItems = $(".project-list-item");
    var taskFilterList = $(".task-filter");
    $(function () {
        $(projectListItems).find("i").addClass("hide");
        $(projectListItems).filter(function () {
            return $(this).attr("value") === currentSettings.projectId;
        }).find("i").removeClass("hide");
        $(taskFilterList).find("i").addClass("hide");
        $(taskFilterList[currentSettings.taskFilterId]).find("i").removeClass("hide");
    });

    var updateList = function (form) {
        var options = {
            url: $(form).attr("action"),
            type: $(form).attr("method"),
            data: $(form).serialize()
        };

        $.ajax(options).done(function (data) {
            var $target = $($(form).attr("data-pgs-target"));
            $target.replaceWith(data);
        });
    }

    var sidebarAction = function (item, hiddenItem, itemList) {
        var selectedId = $(item).attr("value");
        $(hiddenItem).val(selectedId);
        $(itemList).find("i").addClass("hide");
        $(item).find("i").removeClass("hide");
        updateList(".project-list-form");
        return selectedId;
    };

    $(".body-content").on("click", "#addSubTaskButton", function () {
        $("#addSubTask").val($("#ProjectTask_Id").val());
        $("#editTaskButton").click();
        return false;
    });

    $(".body-content").on("click", ".project-list-item", function () {
        $("#page").val(1); // reset display back to page 1
        currentSettings.projectId = sidebarAction($(this), $("#projectId"), $(projectListItems));
        // need to disable Create button if All projects selected
        setCreateTaskButton();
        return false;
    });

    $(".body-content").on("click", ".task-filter", function () {
        $("#page").val(1); // reset display back to page 1
        sidebarAction($(this), $("#taskFilterId"), $(taskFilterList));
        return false;
    });

    $(".body-content").on("click", ".pagination-list-item", function () {
        sidebarAction($(this), $("#page"), null);
        return false;
    });

    $(".body-content").on("click", ".comment-edit-button", function () {
        $(this).addClass("hide").siblings(".comment-cancel-button").removeClass("hide");
        var editSection = $(this).parents(".comment-edit-section");
        $(editSection).find(".task-comment").addClass("hide");
        $(editSection).find(".comment-task-edit").removeClass("hide");
        $(".comment-task-text").each(function () {
            $(this).css("height", "auto").css("height", this.scrollHeight + this.offsetHeight);
        });
    });
    $(".body-content").on("click", ".comment-cancel-button", function () {
        $(this).addClass("hide").siblings(".comment-edit-button").removeClass("hide");
        var editSection = $(this).parents(".comment-edit-section");
        var taskComment = $(editSection).find(".task-comment");
        var taskRollback = $(editSection).find("#rollbackComment");
        $(taskComment).removeClass("hide").siblings(".comment-task-edit").addClass("hide")
            .find("textarea").val($(taskRollback).val());
    });

    $(".body-content").on("click", ".comment-delete-button", function () {
        var editSection = $(this).parents(".comment-edit-section");
        if (editSection.hasClass("task-comment-deleted")) {
            $(this).siblings("input").val(false);
            $(editSection).find(".comment-edit-button").removeClass("hide");
            $(editSection).find(".task-comment-date").removeClass("task-comment-deleted");
            $(editSection).removeClass("task-comment-deleted");
        } else {

            $(this).siblings("input").val(true);
            $(editSection).find(".comment-edit-button").addClass("hide");
            $(editSection).find(".task-comment-date").addClass("task-comment-deleted");
            $(editSection).addClass("task-comment-deleted");
        }
    });

    $(".body-content").on("click", ".task-description-edit-button", function () {
        $(this).addClass("hide").siblings(".task-description-cancel-button").removeClass("hide");
        var editSection = $(this).parents(".task-description-section");
        $(editSection).find(".task-description").addClass("hide");
        $(editSection).find(".task-description-edit").removeClass("hide");
        $(".task-description-text").each(function () {
            $(this).css("height", "auto").css("height", this.scrollHeight + this.offsetHeight);
        });
    });

    $(".body-content").on("click", ".task-description-cancel-button", function () {
        $(this).addClass("hide").siblings(".task-description-edit-button").removeClass("hide");
        var editSection = $(this).parents(".task-description-section");
        var taskDescription = $(editSection).find(".task-description");
        var rollbackDescription = $(editSection).find("#rollbackDescription");
        $(taskDescription).removeClass("hide").siblings(".task-description-edit").addClass("hide")
            .find("textarea").val($(rollbackDescription).val());
    });

    $(".body-content").on("change", "#ProjectTask_TaskStatusId", function () {
        var completedStatus = "3"; // index value of Completed in TaskStatus enum
        if ($(this).val() === completedStatus) {
            $("#addSubTaskButton").addClass("hide");
        } else {
            $("#addSubTaskButton").removeClass("hide");
        }
		return false;
    });

    var toggleSettingsSidebar = function () {
        if ($(".settings-sidebar").hasClass("on")) {
            $(".settings-sidebar").removeClass("on");
        } else {
            $(".settings-sidebar").addClass("on");
        }
        return false;
    }

    $(".navbar-content").on("click", ".settings-link", function () {
        toggleSettingsSidebar();
        return false;
    });

    $(".sidebar").on("click", ".settings-close", function() {
        toggleSettingsSidebar();
        return false;
    });
    
    // set height of project task title
    $(function () {
        $(".task-title").css("height", "auto").css("height", $(".task-title")[0].scrollHeight.toString() + "px");
    });

    $(window).on("load", function() {
        $("#cp-modalCookiePolicy").modal("show");
    });

    var saveSelectedCookies = function (data) {
        var url = "CookiePolicy/SaveSelected";
        var antiForgeryToken = $("#cp-modalCookiePolicy input[name='__RequestVerificationToken']").val();
        data.__RequestVerificationToken = antiForgeryToken;
        event.preventDefault();
        $.ajax({
            type: "POST",
            url: url,
            data: data,
            success : function() {
                location.href = document.referrer;
            }
        });
        return false;
    }

    $(function() {
        $("#cp-accept-recommended-btn-handler").on("click", function() {
            var data = {
                CookiePolicyAccepted: true,
                MarketingCookieEnabled: true,
                AnalyticsCookieEnabled: true
            }
            saveSelectedCookies(data);
        });
        return false;
    });

    $(function() {
        $("#cp-selection-btn-handler").on("click", function() {
            //todo change the hard coded settings in this function
            var data = {
                CookiePolicyAccepted: true,
                MarketingCookieEnabled: false,
                AnalyticsCookieEnabled: true
            }
            saveSelectedCookies(data);
        });
        return false;
    });
})