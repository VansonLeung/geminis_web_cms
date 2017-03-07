$(document).ready(function() {
  $('iframe.if-flexible').iFrameResize({log: true, interval: 32, resizeFrom: 'parent', checkOrigin: false});
})

var iframeLoaded = function(e)
{
  console.log(e);
}

// This will popup any html URLs with given height
window.popupIframe = function(url, height)
{
	window.____p = new Popelt({
	    contentType: 'iframe',
	    loadUrl: url,
	    iframeWidth: '100%',
	    iframeHeight: height || '500px'
	});
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