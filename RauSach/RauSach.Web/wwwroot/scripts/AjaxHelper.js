var AjaxHelper = function () {
    this.message_ajax_error = "Load data error";
    this.typeGet = "GET";
    this.typePost = "POST";
    this.jsonType = "json";
    this.htmlType = "html";
}

AjaxHelper.prototype.excuteAjax_ReturnData = function (dataType, type, url, inputData, message) {
    dataType = typeof dataType !== 'undefined' ? dataType : ajaxHelper.jsonType;
    type = typeof type !== 'undefined' ? type : ajaxHelper.typeGet;
    message = typeof message !== 'undefined' ? message : ajaxHelper.message_ajax_error;
    var returnData;
    $.ajax({
        dataType: dataType,
        type: type,
        url: url,
        data: inputData,
        async: false,
        cache: false,
        beforeSend: function () {
            $('.ajax-loading').show();
        },
        success: function (data) {
            returnData = data;
            $('.ajax-loading').hide();
        },
        error: function (xhr, status, error) {
            console.log("Common.ExcuteAjax_ReturnData: url:" + url + ' ' + xhr.responseText);
            returnData = null;
            $('.ajax-loading').hide();
        }
    });
    return returnData;
};

AjaxHelper.prototype.excuteAjax_ReturnDataJson = function (url, inputData, message) {
    return ajaxHelper.excuteAjax_ReturnData(ajaxHelper.jsonType, ajaxHelper.typeGet, url, inputData, message);
};
AjaxHelper.prototype.excuteAjax_ReturnDataHtml = function (url, inputData, message) {
    return ajaxHelper.excuteAjax_ReturnData(ajaxHelper.htmlType, ajaxHelper.typeGet, url, inputData, message);
};
AjaxHelper.prototype.excutePostAjax_ReturnDataHtml = function (url, inputData, message) {
    return ajaxHelper.excuteAjax_ReturnData(ajaxHelper.htmlType, ajaxHelper.typePost, url, inputData, message);
};
AjaxHelper.prototype.excutePostAjax_ReturnDataJson = function (url, inputData, message) {
    return ajaxHelper.excuteAjax_ReturnData(ajaxHelper.jsonType, ajaxHelper.typePost, url, inputData, message);
};

var ajaxHelper = new AjaxHelper();

