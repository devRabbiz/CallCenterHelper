/// <reference path="jquery-1.9.1.js" />
function getJonsp(host, url, data, success) {
    /// <summary>
    /// 尚未正式启用。待确定
    /// </summary>
    /// <param name="host"></param>
    /// <param name="url"></param>
    /// <param name="data"></param>
    /// <param name="success"></param>
    /// <returns type=""></returns>
    return $.getJSON(host + url + '?callback=?', data, success);
}

function $getJsonp(url, data, async, timeout) {
    return $.ajax({
        async: true,
        url: url,
        type: 'GET',
        dataType: 'jsonp',
        data: data, //请求数据
        timeout: 3000
    });
}

var XMLHTTPVERSIONCACHE = '';
function xmlHttpRequest(url, data, async, success) {
    /// <summary>
    /// 通过Msxml2.XMLHTTP请求
    /// </summary>
    /// <param name="url">链接</param>
    /// <param name="data">数据json格式</param>
    /// <param name="async">true为异步 false同步</param>
    /// <param name="success">请求成功事件</param>
    var xmlHttp = false;
    try {
        if (XMLHTTPVERSIONCACHE != '') {
            xmlHttp = new ActiveXObject(XMLHTTPVERSIONCACHE);
        }
        else {
            xmlHttp = new ActiveXObject("Msxml2.XMLHTTP.6.0");
            XMLHTTPVERSIONCACHE = 'Msxml2.XMLHTTP.6.0';
        }
    }
    catch (e) {
        try {
            xmlHttp = new ActiveXObject("Msxml2.XMLHTTP.3.0");
            XMLHTTPVERSIONCACHE = 'Msxml2.XMLHTTP.3.0';
        }
        catch (e2) {
            try {
                xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                XMLHTTPVERSIONCACHE = 'Microsoft.XMLHTTP';
            }
            catch (e3) {
                xmlHttp = false;
            }
        }
    }

    if (xmlHttp) {
        xmlHttp.open("POST", url, async);
        xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == '4') {
                if (xmlHttp.status == 200) {
                    if (success != undefined) {
                        (success)(xmlHttp.responseText);
                    }
                }
            }
        }
        xmlHttp.send($.param(data));
    }
}

//消息 是否异步
function lenovoTrace(msg, async) {
    if (async == undefined) {
        async = false;
    }
    try{
        xmlHttpRequest('/LenovoTrace.aspx', { act: 'trace', msg: msg }, async, function () { });
    }
    catch (e) { }
}