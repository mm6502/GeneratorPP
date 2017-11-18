
function inProgress(delay, id, baseUrl) {

    var progressbar = $("#progressbar");
    var progressLabel = $(".progress-label");

    var progressUrl = baseUrl + "/Home/GetProgress/" + id;
    var generateUrl = baseUrl + "/Home/Generate/" + id;
    var successUrl = baseUrl + "/Home/Finish";
    var errorUrl = baseUrl + "/Home/Error";


    progressbar.progressbar({
        value: false,
        change: function() {
            var value = progressbar.progressbar("value");
            if (typeof value === "number")
                progressLabel.text(value + "%");
            else
                progressLabel.text("");
        }
    });

    $.ajaxSetup({
        type: "GET",
        cache: false,
        xhrFields: {
            withCredentials: true
        }
    });

    var repeatHandle = window.setInterval(function() {
            $.ajax({
                    type: "GET",
                    url: progressUrl,
                    cache: false,
                    xhrFields: {
                        withCredentials: true
                    }
                })
                .done(function(percent) {
                    progressbar.progressbar("option", "value", percent);
                });
        },
        delay);

    $.fileDownload(generateUrl)
        .done(function() { window.location.href = successUrl })
        .fail(function() { window.location.href = errorUrl });
}
