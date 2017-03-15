<script type="text/javascript">

window.js_qpi_login = function(_callback)
{
	var get_qpi_login_params = function(callback)
	{
		$.get(
		    "/api/session/get_qpi_login_params",
		    function(response) {
		        var json = JSON.parse(response);
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

	var login = function(callback)
	{
		var url = "http://uat.quotepower.com/web/geminis/login.jsp";

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
		    function(data) {
		        var json = JSON.parse(response);
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

	get_qpi_login_params(function(response1) {
		login(function(response2) {
			set_qpi_login_token(response2, function(response3) {
				_callback(response3);
			})
		})
	})
}

</script>