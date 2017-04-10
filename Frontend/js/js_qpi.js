var get_qpi_login_params = function(callback)
{
    console.log("get_qpi_login_params");
    $.post(
        "/api/session/get_qpi_login_params",
        function(response) {
            var json = response;
            if (json.success)
            {
                var data = json.data;
                callback(data);
            }
            else
            {
                Alert("qpi_login_err_1");
            }
        }
    );
}

var keep_alive = function (params, callback) {

    console.log("keep_alive", params);

    var url = params.keep_alive_url;

    var jsessionid = params.jsessionid;

    if (!jsessionid || jsessionid === '')
    {
        callback(null);
    }

    var query = ";jsessionid=" + jsessionid;

    var fullurl = url + query;

    $.get(
        fullurl,
        function (response) {
            var json = response;
            if (typeof response === 'string') {
                json = JSON.parse(response);
            }
            callback(response);
        }
    );
}


var login = function(params, callback)
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

    $.get(
        fullurl,
        function (response) {
            var json = response;
            if (typeof response === 'string')
            {
                json = JSON.parse(response);
            }
            callback(response);
        }
    );
}

var set_qpi_login_token = function(jsession, callback)
{

    $.ajax({
        type: "POST",
        url: "/api/session/set_qpi_login_token",
        // The key needs to match your method's input parameter (case-sensitive).
        data: JSON.stringify(jsession),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(data){
            callback(data);
        },
        failure: function(errMsg) {
            Alert("qpi login err 2");
        }
    });
}



window.js_qpi_login = function(_callback)
{
    get_qpi_login_params(function(response1) {
        login(response1, function(response2) {
            set_qpi_login_token(response2, function(response3) {
                _callback(response3);
            })
        })
    })
}

window.js_qpi_keep_alive = function(_callback)
{
    get_qpi_login_params(function(response1) {
        keep_alive(response1, function(response2) {
            _callback(response2);
        })
    })
}


$(function () {
    window.js_qpi_keep_alive(function (result) {
        console.log("Keep Alive...", result);
    })
})


