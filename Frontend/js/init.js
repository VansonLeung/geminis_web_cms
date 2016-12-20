$(document).ready(function() {
  $('iframe.if-flexible').iFrameResize({log: true, interval: 32, resizeFrom: 'parent'});
})

var iframeLoaded = function(e)
{
  console.log(e);
}