window.jsqpi_get_qpi_login_params = function(callback)
{
    console.log("get_qpi_login_params");
    $.post(
        "/api/session/get_qpi_login_params"
    )
    .done(function (response) { 
        console.log("0002", response);
        var json = response;
        if (json.success)
        {
            var data = json.data;
            callback(data);
        }
        else
        {
            console.log(response);
            callback(null);
        }
    })
    .fail(function (xhr, status, error) {
        console.log("0001", status, error);
        callback(null);
    });
}

window.jsqpi_keep_alive = function (params, callback) {

    console.log("keep_alive", params);

    var url = params.keep_alive_url;

    var jsessionid = params.jsessionid;

    if (!jsessionid || jsessionid === '')
    {
        callback(null);
    }

    var query = ";jsessionid=" + jsessionid;

    var fullurl = url + query;

    $.ajax({
        type: "GET",
        url: fullurl,
        // The key needs to match your method's input parameter (case-sensitive).
        dataType: "jsonp",
        crossDomain: true,
        jsonp: false,
        jsonpCallback: "callback",
        success: function (data) {
            console.log("0004", data);
            callback(data);
        },
        error: function (errMsg) {
            console.log("0003", errMsg);
            callback(null);
        }
    });
}


window.jsqpi_login = function (params, callback)
{

    console.log("login", params);

    var url = params.login_url;

    var domain = params.domain;
    var uid = params.uid;
    var ts = params.ts;
    var env_key = params.env_key;
    var password = params.password;
    var token = params.token;

    var query = "?token=" + token;
    query += "&domain=" + domain;
    query += "&uid=" + uid;
    query += "&ts=" + ts;

    var fullurl = url + query;

    $.ajax({
        type: "GET",
        url: fullurl,
        // The key needs to match your method's input parameter (case-sensitive).
        dataType: "jsonp",
        crossDomain: true,
        jsonp: false,
        jsonpCallback: "callback",
        success: function (data) {
            console.log("0006", data);
            callback(data);
        },
        error: function (errMsg) {
            console.log("0005", errMsg);
            callback(null);
        }
    });
}

window.jsqpi_set_qpi_login_token = function (jsession, callback)
{

    $.ajax({
        type: "POST",
        url: "/api/session/set_qpi_login_token",
        // The key needs to match your method's input parameter (case-sensitive).
        data: JSON.stringify(jsession),
        success: function (data) {
            console.log("0008", data);
            callback(data);
        },
        error: function(errMsg) {
            console.log("0007", errMsg);
            callback(null);
        }
    });
}



window.js_qpi_login = function(_callback)
{
    window.jsqpi_get_qpi_login_params(function (response1) {
        if (response1 == null)
        {
            _callback(null);
            return;
        }
        window.jsqpi_login(response1, function (response2) {
            if (response2 == null) {
                _callback(null);
                return;
            }
            window.jsqpi_set_qpi_login_token(response2, function (response3) {
                _callback(response3);
            })
        })
    })
}

window.js_qpi_keep_alive = function(_callback)
{
    window.jsqpi_get_qpi_login_params(function (response1) {
        if (response1 == null) {
            _callback(null);
            return;
        }
        window.jsqpi_keep_alive(response1, function (response2) {
            _callback(response2);
        })
    })
}


$(function () {
    window.js_qpi_keep_alive(function (result) {
        console.log("Keep Alive...", result);
    })
})


