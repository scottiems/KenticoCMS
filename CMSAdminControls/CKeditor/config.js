/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/
CKEDITOR.editorConfig = function (config) {

    config.extraPlugins = 'CMSPlugins';
    config.uiColor = '#eeeeee';
    config.skin = 'kentico';
    config.enterMode = CKEDITOR.ENTER_BR;
    config.shiftEnterMode = CKEDITOR.ENTER_P;
    config.entities_latin = false;
    config.protectedSource.push(/<script[\s\S]*?<\/script>/gi);   // <SCRIPT> tags.

    config.toolbar_Thinkgate = config.toolbar_Thinkgate_Full =
    [
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', 'SpellChecker', 'Scayt', '-'],
        ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
        ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent', '-'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
        ['Table', 'HorizontalRule', 'SpecialChar', '-'],
        ['Styles', 'Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor', '-']
    ];

    config.toolbar_Full = config.toolbar_Default =
    [
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', 'SpellChecker', 'Scayt', '-'],
        ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
        ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent', '-'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
        ['Table', 'HorizontalRule', 'SpecialChar', '-'],
        ['Styles', 'Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor', '-']
    ];

    config.toolbar_Wireframe =
    [
	    ['Cut', 'Copy', 'PasteText', '-'],
	    ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
	    ['Bold', 'Italic', 'Underline', 'Strike', '-'],
	    ['NumberedList', 'BulletedList', '-'],
	    ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
	    ['QuicklyInsertImage', 'Table', '-'],
	    ['Format']
    ];

    config.toolbar_Basic =
    [
	    ['Bold', 'Italic', '-', 'NumberedList', 'BulletedList']
    ];

    config.toolbar_ProjectManagement =
    [
	    ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', 'RemoveFormat', '-', 'NumberedList', 'BulletedList', '-', 'TextColor', 'BGColor']
    ];

    config.toolbar_BizForm = [
	    ['Source', '-'],
	    ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-'],
	    ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
	    ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
	    ['NumberedList', 'BulletedList', 'Outdent', 'Indent', '-'],
	    ['Anchor', '-'],
	    ['Table', 'HorizontalRule', 'SpecialChar', '-'],
	    ['Styles', 'Format', 'Font', 'FontSize'],
	    ['TextColor', 'BGColor', '-'],
        ['Maximize']
    ];

    config.toolbar_Forum = [
	    ['Bold', 'Italic', '-', 'InsertUrl', 'QuicklyInsertImage', 'InsertQuote', '-', 'NumberedList', 'BulletedList', '-', 'TextColor', 'BGColor']
    ];

    config.toolbar_Newsletter = config.toolbar_Reporting = [
	    ['Source', '-'],
	    ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-'],
	    ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
	    ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
	    ['NumberedList', 'BulletedList', 'Outdent', 'Indent', '-'],
	    ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
	    ['Anchor', '-'],
	    ['QuicklyInsertImage', 'Table', 'HorizontalRule', 'SpecialChar', '-'],
	    ['Styles', 'Format', 'Font', 'FontSize'],
	    ['TextColor', 'BGColor', '-'],
	    ['Maximize']
    ];

    config.toolbar_SimpleEdit = [
	    ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-'],
	    ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
	    ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
	    ['NumberedList', 'BulletedList', 'Outdent', 'Indent', '-'],
	    ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
	    ['Anchor', '-'],
	    ['QuicklyInsertImage', 'Table', 'HorizontalRule', 'SpecialChar', '-'],
	    ['Styles', 'Format', 'Font', 'FontSize'],
	    ['TextColor', 'BGColor', '-'],
	    ['Maximize']
    ];

    config.toolbar_Invoice = [
        ['Source', '-'],
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', 'SpellChecker', 'Scayt', '-'],
        ['Undo', 'Redo', 'Find', 'Replace', 'RemoveFormat', '-'],
        ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-'],
        ['NumberedList', 'BulletedList', 'Outdent', 'Indent', 'Blockquote', 'CreateDiv', '-'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-'],
        ['Table', 'HorizontalRule', 'SpecialChar', '-'],
        ['Styles', 'Format', 'Font', 'FontSize'],
        ['TextColor', 'BGColor', '-'],
        ['Maximize']
    ];

    config.toolbar_Group = [
	    ['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', 'InsertGroupPolls']
    ];

    config.toolbar_Widgets = [
	    ['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', '-'],
	    ['Format', 'Font', 'FontSize'],
	    ['TextColor', 'BGColor']
    ];

    config.toolbar_Disabled = [
        ['Maximize']
    ];

    config.scayt_customerid = '1:vhwPv1-GjUlu4-PiZbR3-lgyTz1-uLT5t-9hGBg2-rs6zY-qWz4Z3-ujfLE3-lheru4-Zzxzv-kq4';
};
