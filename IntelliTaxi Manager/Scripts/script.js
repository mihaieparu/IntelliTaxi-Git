var data;

function returnPosition(pos) {
    var loc = { Lat: pos.coords.latitude.toString().replace(".", ","), Long: pos.coords.longitude.toString().replace(".", ","), Name: "", SupportsMap: true };
    data = "Lat=" + encodeURIComponent(loc.Lat) + "&Long=" + encodeURIComponent(loc.Long) + "&Name=" + encodeURIComponent(loc.Name) + "&SupportsMap=" + encodeURIComponent(loc.SupportsMap) + "&DType=" + encodeURIComponent(WURFL.form_factor.toString().replace("-", "").replace(" ", ""));
    var date = new Date();
    date.setSeconds(date.getSeconds() + 60);
    setCookie("Location", loc.Lat + ";" + loc.Long, date);
}

function AjaxUpdateUserData(callback) {
    if (!navigator.geolocation) {
        data = null;
    }
    else {
        navigator.geolocation.getCurrentPosition(returnPosition, null, {enableHighAccuracy: true, timeout: 5000});
    }
    setTimeout(function () {
        $.ajax({
            type: "POST",
            url: "/AJAX/UpdateUserDataAsync",
            data: (data ? data : "")
        }).success(
            function(result) { 
                var date = new Date();
                if (result == "ready") {
                    fails = 0;
                    console.log("User updated - " + date.getDay().toString() + (date.getMonth() + 1).toString() + date.getFullYear().toString() + "@" + date.getHours().toString() + date.getMinutes().toString() + date.getSeconds().toString());
                    if (callback) { callback(1); }
                }
                else {
                    var date = new Date();
                    console.error("Fail " + ++fails + " - " + date.getDay().toString() + (date.getMonth() + 1).toString() + date.getFullYear().toString() + "@" + date.getHours().toString() + date.getMinutes().toString() + date.getSeconds().toString());
                    if (callback) { callback(0); }
                }
            }
        ).error(
            function () {
                var date = new Date();
                console.error("Fail " + ++fails + " - " + date.getDay().toString() + (date.getMonth() + 1).toString() + date.getFullYear().toString() + "@" + date.getHours().toString() + date.getMinutes().toString() + date.getSeconds().toString());
                if (callback) { callback(0); }
            }
        );
    }, 5000);
}

function AjaxDisconnect() {
    $.ajax({
        type: "POST",
        url: "/AJAX/DisconnectAsync",
        async: false
    });
}

function setCookie(c_name, value, exdate) {
    var c_value = escape(value) + "; expires=" + exdate.toUTCString();
    document.cookie = c_name + "=" + c_value;
}
function getCookie(c_name) {
    var c_value = document.cookie;
    var c_start = c_value.indexOf(" " + c_name + "=");
    if (c_start == -1) {
        c_start = c_value.indexOf(c_name + "=");
    }
    if (c_start == -1) {
        c_value = null;
    }
    else {
        c_start = c_value.indexOf("=", c_start) + 1;
        var c_end = c_value.indexOf(";", c_start);
        if (c_end == -1) {
            c_end = c_value.length;
        }
        c_value = unescape(c_value.substring(c_start, c_end));
    }
    return c_value;
}

function hide(element) {
    $(element).animate({opacity:0, height:0}, function () { $(element).remove() });
}