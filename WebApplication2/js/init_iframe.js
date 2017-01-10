function resizeMe()
{
	if (!window.parentIFrame)
	{
		window.parent.postMessage('[iFrameResizerChild]Ready','*');
	}
	if (window.parentIFrame)
	{
		window.parentIFrame.size();
	}
	setTimeout(function()
	{
		resizeMe();
	}, 4000);
}

resizeMe();
