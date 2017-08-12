// Write your Javascript code.
$(function() {
    var createButton = $("#createButton");
    $(function () {
        var btnhref = $(createButton).attr("href");
        var ending = btnhref.substring(btnhref.lastIndexOf("/"));
        if (ending === "/0") {
            $(createButton).addClass("disabled");
        } else {
            $(createButton).removeClass("disabled");
        }
    });

    var currentProjectId = $("#projectId").val();
    var icon = "<i class=\"fa fa-angle-right\"></i>";
    $(function () {
        var projectListItem = $(".projectListItem")[currentProjectId];
        var currentText = $(projectListItem).text();
        $(projectListItem).html(icon + currentText);
    });


})