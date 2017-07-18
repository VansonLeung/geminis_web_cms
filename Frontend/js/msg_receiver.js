function receiver(message) {
    // var trusteddomain = "http://fiddle.jshell.net";
    // if (message.origin == trusteddomain) 
    {
        var data = null;
        try
        {
            data = JSON.parse(message.data);
        }
        catch (e)
        {
            return;
        }
        var msg = data;


        /**
         * function: change a particular iframe's URL inside the page
         * action: iframe_change_url
         * target: iframe ID
         * url: URL of the iframe to change to
         */
    	if (msg.action === "iframe_change_url")
    	{
		    var y = document.body.scrollTop; // save parent y to undo jump to top of IFRAME

		    var iframe = document.getElementById(msg.target).contentWindow;
		    iframe.location.href = msg.url;

		    document.body.scrollTop = y; // reset y

    	}


    	if (msg.action === "iframe_set_anchor") {
    	    console.log("SET ANCHOR..." + msg);
    	    var anchor = msg.anchor;
    	    var offset = msg.offset;
    	    var target = msg.target || "iframe#TTLIframe";
    	    if (anchor) {
    	        var currentHref = window.location.href;
    	        if (currentHref.indexOf("#") > 0) {
    	            window.location.href = currentHref.substring(0, currentHref.indexOf("#")) + "#top";
    	        }
    	        window.location.href = "#" + anchor;
    	    }
    	    if (offset != undefined && offset != null) {
    	        window.scrollTo(0, $(target).offset().top + offset);
    	    }
    	}


        /**
            * function: refresh a particular iframe inside the page
            * action: iframe_refresh
            * target: iframe ID
            */
    	else if (msg.action === "iframe_refresh") {
    	    document.getElementById(msg.target).contentWindow.location.reload(true);
    	}


    	    /**
             * function: scroll page to element by element ID
             * action: jump_to_element
             * target: element ID
             */
    	else if (msg.action === "jump_to_element") {
    	    var e = document.getElementById(msg.target);
    	    if (!!e && e.scrollIntoView) {
    	        e.scrollIntoView();
    	    }
    	}


    	    /**
             * function: scroll page to element by height pixel
             * action: jump_by_px
             * target: height in integer (or a number / string without suffix "px")
             */
    	else if (msg.action === "jump_by_px") {
    	    var y = document.body.scrollTop;
    	    var px = Number(msg.target);
    	    y += px;
    	    document.body.scrollTop = y;
    	}


    	    /**
             * function: trigger Geminis to pop-up an iframe with URL
             * action: popup_url
             * target: iframe popup height in string format with "px" suffix e.g.: "500px"
             * url: pop-up URL
             */
    	else if (msg.action === "popup_url") {
    	    window.popupIframe(msg.url, msg.target);
    	}



    	    /**
             * function: trigger Geminis to pop-up an iframe with URL
             * action: popup_url
             * target: iframe popup height in string format with "px" suffix e.g.: "500px"
             * url: pop-up URL
             */
    	else if (msg.action === "popup_url_fullscreen") {
    	    window.popupIframe(msg.url, msg.target, true, true);
    	}



    	    /**
             * function: trigger Geminis to pop-up an iframe with URL
             * action: close_popup
             * target: iframe popup height in string format with "px" suffix e.g.: "500px"
             * url: pop-up URL
             */
    	else if (msg.action === "close_popup") {
    	    window.closePopupIframe();
    	}




    	    /**
             * function: trigger Geminis page to run keep-alive
             * action: keep_alive_gem
             */
    	else if (msg.action === "keep_alive_gem") {
    	    $.post("/api/session/api_sso_keepalive");
    	}

    	    /**
             * function: trigger Geminis page to run keep-alive
             * action: keep_alive_gem
             */
    	else if (msg.action === "heart_beat_gem") {
    	    $.post("/api/session/api_sso_heartbeat");
    	}


    	    /**
             * function: redirect the page to another url
             * action: redirect_to_url
             * url: URL of the page to be redirected to
             */
    	else if (msg.action === "redirect_to_url") {
    	    window.location.href = msg.url;
    	}



    	    /**
             * function: redirect the page to another url
             * action: redirect_to_url
             * url: URL of the page to be redirected to
             */
    	else if (msg.action === "force_expire") {
    	    $.ajax({
    	        type: "POST",
    	        url: "/api/session/api_sso_force_expire",
    	        // The key needs to match your method's input parameter (case-sensitive).
    	        data: JSON.stringify({}),
                dataType: 'json',
                success: function (data) {
                    window.location.href = msg.url;
                },
    	        error: function (errMsg) {

    	        }
    	    });
    	}


    	    /**
             * function: refresh the whole page
             * action: refresh
             */
    	else if (msg.action === "refresh") {
    	    window.location.reload(true);
    	}



    	else if (msg.action === "scrolltoChart") {
    	    $('html,body').animate({
    	        scrollTop: $("#i0").offset().top
    	    },
                'slow');
    	}





    	    /**
             * function: trigger Geminis to pop-up an iframe with URL
             * action: popup_url
             * target: iframe popup height in string format with "px" suffix e.g.: "500px"
             * url: pop-up URL
             */
    	else if (msg.action === "showLoading") {
    	    window.turnLoader("c");
    	}






    	    /**
             * function: trigger Geminis to pop-up an iframe with URL
             * action: popup_url
             * target: iframe popup height in string format with "px" suffix e.g.: "500px"
             * url: pop-up URL
             */
    	else if (msg.action === "hideLoading") {
    	    window.turnLoader();
    	}



    }
}

window.addEventListener('message', receiver, false);
