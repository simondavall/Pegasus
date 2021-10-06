$(function() {
    $(window).on("load", function() {
        $("#cp-modalCookiePolicy").modal("show");
    });

    var saveSelectedCookies = function (data) {
        var url = "/CookiePolicy/SaveSelected";
        var antiForgeryToken = $("#cp-modalCookiePolicy input[name='__RequestVerificationToken']").val();
        data.__RequestVerificationToken = antiForgeryToken;
        event.preventDefault();
        $.ajax({
            type: "POST",
            url: url,
            data: data,
            success : function() {
                location.href = document.location.href;
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
            var data = {
                CookiePolicyAccepted: true,
                MarketingCookieEnabled: $("#marketing-cookies").is(":checked"),
                AnalyticsCookieEnabled: $("#analytics-cookies").is(":checked")
            }
            saveSelectedCookies(data);
        });
        return false;
    });
})

