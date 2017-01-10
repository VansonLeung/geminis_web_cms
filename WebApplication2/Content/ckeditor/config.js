/**
 * @license Copyright (c) 2003-2016, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.allowedContent = true;
    config.extraPlugins = 'video,widgetbootstrap';
    config.extraAllowedContent = 'video [*]{*}(*);source [*]{*}(*);script [*]{*}(*);';
    config.filebrowserBrowseUrl = '/Content/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '/Content/ckfinder/ckfinder.html?type=Images';
    config.filebrowserUploadUrl = '/fileupload/UploadNow';
    config.filebrowserImageUploadUrl = '/image/UploadNow';
    config.filebrowserVideoUploadUrl = '/VideoUpload/UploadPage';
};
