$(document).ready(function() {
  $('iframe.if-flexible').iFrameResize({log: true, interval: 32, resizeFrom: 'parent', checkOrigin: false});
})

var iframeLoaded = function(e)
{
  console.log(e);
}

// This will popup any html URLs with given height
window.popupIframe = function(url, height, is_fullscreen, has_close_button)
{
	var options = {
	    contentType: 'iframe',
	    loadUrl: url,
	    iframeWidth: '100%',
	    iframeHeight: height || '500px',
	    closeButton: false,
	};

	if (is_fullscreen)
	{
		options.width = '80%';
		options.height = '80%';
		options.iframeWidth = '100%';
		options.iframeHeight = '' + window.screen.height - 150 + 'px';
		options.closeButton = true;
	}

	window.____p = new Popelt(options);
	// p.addCloseButton();
	window.____p.show();
}

window.closePopupIframe = function()
{
	if (window.____p)
	{
		window.____p.close();
		window.____p = null;
	}
}