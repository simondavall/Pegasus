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
})