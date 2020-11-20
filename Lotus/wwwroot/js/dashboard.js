function CheckCredentials() {
    UpdateLogger("Checking twitter credentials");
    $.ajax({
        url: "/Index?handler=CheckCredentials",
        type: "GET",
        contentType: "application/json",
        success: function(data) {
            UpdateLogger(data === "good" ? "Twitter credentials OK" : "Twitter credentials FAIL");
            GetAccountData();
        }
    });
}

function GetAccountData() {
    UpdateLogger("Checking account");
    $.ajax({
        url: "/Index?handler=AccountData",
        type: "GET",
        contentType: "application/json",
        success: function (data) {
            UpdateLogger(data.username != null ? "Twitter account OK" : "Twitter account FAIL");
            $("#card-username").text(data.username);
            $("#total-followers").text(data.followers);
            $("#total-following").text(data.following);
        }
    });
}

function UpdateLogger(text) {
    $("#log-area").val($("#log-area").val() + `\n${text}.`);
}
$(document).ready(function() {
    //CheckCredentials();

    $("#twitter-following-text-button").click(() => {
        $("#twitter-following-modal").modal("hide");
        UpdateLogger("Starting auto following");
        var text = $("#twitter-following-text-textarea").val();
        $.ajax({
            url: "/Index?handler=TwitterAutoFollowing",
            type: "GET",
            data: {text:text},
            success: function(data) {
                UpdateLogger(data.username != null ? "Starting SUCCESS" : "Starting FAIL");
                var counter = 0;
                var i = window.setInterval(function() {
                        GetAccountData();
                        counter++;
                        if (counter === 100) {
                            clearInterval(i);
                        }
                    },
                    30000);
            }
        });

    });
});