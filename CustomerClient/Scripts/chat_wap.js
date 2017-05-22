/// <reference path="jquery-1.7.1.min.js" />

//本chat的操作
function lenovoTrace(msg, async) {
    try {
        if (async == undefined) {
            async = true;
        }
        xmlHttpRequest('/Lenovo/Trace', { message: chatCustomerID + "|" + msg }, async);
    }
    catch (e) { }
}

String.format = function () {
    if (arguments.length == 0)
        return null;

    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
}
/*
函数：格式化日期
参数：formatStr-格式化字符串
返回：格式化后的日期
*/
Date.prototype.format = function (formatStr) {
    var date = this;
    var zeroize = function (value, length) {
        if (!length) length = 2;
        value = new String(value);
        for (var i = 0, zeros = ''; i < (length - value.length) ; i++) {
            zeros += '0';
        }
        return zeros + value;
    };

    return formatStr.replace(/"[^"]*"|'[^']*'|\b(?:d{1,4}|M{1,4}|yy(?:yy)?|([hHms])\1?|[lLZ])\b/g, function ($0) {
        switch ($0) {
            case 'dd': return zeroize(date.getDate());
            case 'MM': return zeroize(date.getMonth() + 1);
            case 'yyyy': return date.getFullYear();
            case 'HH': return zeroize(date.getHours());
            case 'mm': return zeroize(date.getMinutes());
            case 'ss': return zeroize(date.getSeconds());
        }
    });
}

function sendXMLRequest(url, params, callback) {
    var request = null;
    if (window.ActiveXObject) {
        var vers = ["MsXml2.XMLHTTP.6.0", "MsXml2.XMLHTTP.3.0", "MsXml2.XMLHTTP", "Microsoft.XMLHTTP"];
        for (var ii = 0; ii <= vers.length - 1; ii++) {
            try {
                request = new ActiveXObject(vers[ii]);
                break;
            }
            catch (e) { }
        }
    } else if (window.XMLHttpRequest) request = new XMLHttpRequest();
    if (request == null) return false;
    request.open("POST", url, true);
    request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
    request.onreadystatechange = function () {
        if (request.readyState == 4 && request.status == 200) {
            callback(request.responseText);
        }
    }

    // 发送请求
    request.send($.param(params));
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
        xmlHttp.setRequestHeader("X-Requested-With", "XMLHttpRequest");
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

//访客唯一编号
var chatCustomerID = '';

var chatTab = function () {
    var _this = this;
    var timerHandle = null, timerHandle2 = null;
    var customerID = chatCustomerID;
    var second = 0, minute = 0, hour = 0, closeSecond = 3, errorTime = 0;
    var strUserData = $("#ClientUserData").val(), objUserData = eval('(' + strUserData + ')');
    var preText = $("#RabitPreText").val();
    // 客户端连接状态
    var arrStatus = { connected: 0, waiting: 1, chatting: 2, closed: 3 }
    var currentStatus = arrStatus.connected;
    var sysTitle = "<span class='Alert'><b>系统 $time$</b></span><br /> ";

    // 显示欢迎话术
    this.showWelcomeText = function () {
        var url = String.format('/Chat/GetWelcomeText?ss={0}', customerID);
        var params = { enterID: objUserData.EnterID };
        sendRequest(url, params, function (data) {
            var $p = $("#pArea"), msg = '';
            if (data && data.length > 0) {
                $(data).each(function (index) {
                    msg = data[index] + "<br/>";
                });
                $p.append(formatMsg(msg));
                //$p.append(formatMsg("<font color=red>正在建立连接中，请耐心等待......</font>"));
                _this.connectionServer();
            } else {
                $p.append(formatMsg("<font color=red>请访问 <a href='http://support1.lenovo.com.cn'>http://support1.lenovo.com.cn</a></font>"));
            }
        });
    }

    this.connectionServer = function () {
        lenovoTrace('connectionServer');
        var url = String.format('/Chat/ConnectionServer?ss={0}', customerID);
        var params = { sessionID: customerID, queryString: strUserData };
        sendRequest(url, params, function (data) {
            if (data != "OK.") {
                //$("#liHotKey").html("<font color=red> 正在连接消息服务器，请耐心等候……</font><br>");
                timerHandle = setTimeout(_this.connectionServer, 1500);
            }
            else {
                clearTimeout(timerHandle);
                //$("#pArea").append(formatMsg("<font color=red> 欢迎进入联想Web在线支持中心，转接中，请稍候……</font>"));
                currentStatus = arrStatus.waiting;
                setTimeout(_this.getInfo, 1000);
                setTimeout(_this.logBrowser, 1000);
            }
        });
    }

    // 获取版本
    this.logBrowser = function () {
        var br = $.browser, name = '未知', msg = '';
        if (br) {
            if (br.msie) name = 'IE';
            else if (br.mozilla) name = '火狐';
            else if (br.opera) name = '欧朋';
            else if (br.webkit) name = '谷歌';
            else if (br.safari) name = '苹果';
        }
        msg = String.format('$$BROWSER$$用户当前使用的浏览器：{0} {1}', name, br.version);
        _this.sendMessage(msg);
    }

    // 获取聊天消息
    this.getInfo = function () {
        var url = String.format('/Chat/GetInfo_wap?ss={0}', customerID);
        var params = { sessionID: customerID };
        sendRequest(url, params, function (data) {
            if (data) {
                var $p = $("#pArea");
                if (data.MsgList) {
                    for (var i = 0; i < data.MsgList.length; i++) {
                        var msg = data.MsgList[i];
                        $p.append(msg);
                        $p[0].scrollTop = $p[0].scrollHeight;
                    }
                }
                switch (data.Status) {
                    case 1:
                        _this.waitForService();
                        timerHandle = setTimeout(_this.getInfo, 1000);
                        break;
                    case 2:
                        if (preText.length > 0) {
                            var text = unescape(preText);
                            text = text.replace(/<\/?(script)[^>]*>/ig, "");
                            var ntext = text.replace(/<\/?(p|font|strong|em)[^>]*>/ig, "");
                            if (ntext.length > 0)
                                _this.sendMessage(text);
                            preText = '';
                        }
                        $("#btnSend").removeAttr('disabled');
                        if (!timerHandle2) {
                            _this.getChatDuration();
                            $("#liHotKey").html("当前发送快捷键 Enter");
                        }
                        timerHandle = setTimeout(_this.getInfo, 1000);
                        break;
                    case -1:// 关闭聊天计时和信息获取
                        currentStatus = arrStatus.closed;
                        clearInterval(timerHandle);
                        clearTimeout(timerHandle2);
                        //$("#pArea").append(sysTitle.replace('$time$', new Date().format("HH:mm:ss")));
                        //$p.append("<div class='MsgContent'><font color='red'>坐席已离开，聊天结束。请您关闭窗口。</font></div>");
                        $p.append(formatMsg("<font color='red'>网络异常，服务已中断。</font>"))
                        $p[0].scrollTop = $p[0].scrollHeight;
                        $("#btnSend").attr('disabled', 'disabled');
                        break;
                }
            }
        }).error(function () {
            timerHandle = setTimeout(_this.getInfo, 1000);
        });
    }

    // 聊天计时
    this.getChatDuration = function () {
        second++;
        if (second == 60) { second = 0; minute += 1; }
        if (minute == 60) { minute = 0; hour += 1; }
        var data = String.format("{0}:{1}:{2}", checkNumber(hour), checkNumber(minute), checkNumber(second));
        $("#spTime").html(data);
        timerHandle2 = setTimeout(_this.getChatDuration, 1000);

        function checkNumber(n) {
            return (n < 10) ? '0' + n : n;
        }
    }

    this.waitForService = function () {
        var url = String.format('/Chat/WaitForService?ss={0}', customerID);
        var params = { sessionID: customerID, enterID: objUserData.EnterID };
        sendRequest(url, params, function (data) {
            var $ph = $("#liHotKey"), msg = '';
            if (data.Code < 0) msg = '未能获取到排队信息。';
            else if (data.d > 0) {
                msg = String.format("队列目前有{0}位客户等候服务"
                    , data.d > 5 ? 5 : data.d);
            }
            if (msg && msg.length > 0) $ph.html(msg);
        });
    }

    this.sendMessage = function (msg) {
        if (currentStatus == arrStatus.closed) alert("当前对话不可用，请检查是否连接到服务器。");
        msg = msg.replace(/<\/?(script)[^>]*>/ig, "");
        var url = String.format('/Chat/SendMessage?ss={0}', customerID);
        var params = { sessionID: customerID, message: msg };
        if (msg == null) return;
        lenovoTrace(msg);
        if (msg.length < 1024)
            sendRequest(url, params, function (data) { });
        else {
            try {
                sendXMLRequest(url, params, function (data) { });
            }
            catch (e) {
                if (window.console) window.console.log(e.message);
            }
        }
    }

    this.leftChat = function () {
        var url = String.format('/Chat/LeftChat?ss={0}', customerID);
        var params = { sessionID: customerID };
        sendRequest(url, params, function (data) {
            window.opener = "";
            window.open("", "_self");
            window.close();
        });
        lenovoTrace('leftChat', false);
    }

    this.updateTypingStatus = function (status) {
        if (currentStatus == arrStatus.closed) return;
        var url = String.format('/Chat/UpdateTypingStatus?ss={0}', customerID);
        var params = { sessionID: customerID, isStarted: status };
        sendRequest(url, params, function () { });
    }

    function sendRequest(url, params, callback) {
        return $.ajax({
            url: url,
            data: params,
            type: 'POST',
            error: function (e) {
                if (errorTime >= 0) errorTime++;
                if (errorTime > 5) {
                    errorTime = -1;
                    //$("#pArea").append(sysTitle.replace('$time$', new Date().format("HH:mm:ss")));
                    //$("#pArea").append("<div class='MsgContent'><font color='red'>温馨提示：检测到您的网络状态不太稳定，可能已断开连接。</font></div>"); 
                    $("#pArea").append(formatMsg("<font color='red'>温馨提示：检测到您的网络状态不太稳定，可能已断开连接。</font>"));
                }
            },
            success: function (data) {
                errorTime = 0;
                callback(data);
            }
        });
    }
}

function formatMsg(msg) {
    var time = new Date().format("HH:mm:ss");
    //var html = '<div class="AgentMessageBox"><div class="linebox"><div class="line"></div><div class="Alert"><b>系统</b></div><div class="line line_l"></div><div class="clear"></div></div><div class="MsgContent">' + msg + '</div></div>';
    var html = '<div class="waiterBox"><div class="img"><img src="~/Content/mobile/images/waiter.png"></div><div class="con"><img src="~/Content/mobile/images/1.jpg"><p>' + msg + '</p></div></div>';
    return html;
}