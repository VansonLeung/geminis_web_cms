function receiver(message) {
    // var trusteddomain = "http://fiddle.jshell.net";
    // if (message.origin == trusteddomain) 
    {
    	var msg = JSON.parse(message.data);

    	if (msg.action === "iframe_change_url")
    	{
		    var y = document.body.scrollTop; // save parent y to undo jump to top of IFRAME

		    var iframe = document.getElementById(msg.target).contentWindow;
		    iframe.location.href = msg.url;

		    document.body.scrollTop = y; // reset y

    	}
    	else if (msg.action === "iframe_refresh")
    	{
    		document.getElementById(msg.target).contentWindow.location.reload(true);
    	}
    	else if (msg.action === "jump_to_element")
    	{

    	}
    	else if (msg.action === "popup_url")
    	{
    		window.popupIframe(msg.url, msg.target);
    	}
    	else if (msg.action === "keep_alive_gem")
    	{
    		$.post("/api/keep_alive");
    	}
    	else if (msg.action === "redirect_to_url")
    	{
    		window.location.href = msg.url;
    	}
    	else if (msg.action === "refresh")
    	{
    		window.location.reload(true);
    	}
    }
}

window.addEventListener('message', receiver, false);
