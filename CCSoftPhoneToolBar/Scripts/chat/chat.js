/// <reference path="chat_interface.js" />
/// <reference path="chat_api.js" />

//本chat的操作

//应急登录
var isemergency = true;
//业务系统连接
var Business_AppUrl = '';

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

function tabboxEventAll() {
    $(".tabbox").each(function () {
        tabboxEvent(this);
    });
}
//1. tab 切换效果
function tabboxEvent(ele) {
    var liarray = $(ele).find(".tab");
    var labcontentarray = $(ele).find(".contentitem");
    liarray.each(function (index) {
        $(this).click(function () {
            labcontentarray.removeClass("selected");
            $(labcontentarray[index]).addClass("selected");

            liarray.removeClass("selected");
            $(liarray[index]).addClass("selected");

            // 快捷键描述
            var _win = $(labcontentarray[index]).children("iframe")[0].contentWindow;
            if (_win) {
                var sendHotKey = top.getQkSend();
                var keys = ["Enter", "Ctrl + Enter", "Shift + Enter", "Alt + S"];
                _win.$("#liHotKey").html("当前发送快捷键 " + keys[sendHotKey]);

                _win.$("#iframe_txtMessage").focus();
            }
        });

        if ($(liarray[index]).hasClass("selected")) {
            labcontentarray.removeClass("selected");
            $(labcontentarray[index]).addClass("selected");
        }
    });
    if (liarray.length > 0) {
        var len = liarray.filter(".selected").length;
        if (len == 0)
            $(liarray[0]).click();
    }
}

// 模式对话框
function showDialog(url, width, height, isTopWindow) {
    if (!width) width = 900;
    if (!height) height = 500;
    var size = 'dialogHeight:' + height + 'px;dialogWidth:' + width + 'px;edge:raised;resizable:yes;scroll:yes;status:no;center:yes;help:no;minimize:no;maximize:yes;';
    var win = window;
    if (isTopWindow) win = window.top;
    return win.showModalDialog(url, 'window', size);
}
function closeWindow() {
    var isIE = navigator.appName == "Microsoft Internet Explorer";
    var win = top.window;
    if (!isIE) win.close();
    else {
        win.opener = "";
        win.open("", "_self");
        win.close();
    }
}

function eventAccepted(ticketID, interactionID, userData) {
    var data = JSON.parse(userData);
    var type = Object.prototype.toString.apply(data);
    if (type == '[object String]')
        data = eval(data);
    SPhone_Chat.Init(data);
    SPhone_Chat.CustomerName = escape(SPhone_Chat.CustomerName);
    var jsonData = JSON.stringify(SPhone_Chat);
    //var jsonData = '';
    var viewUrl = String.format('/Chat/ChatTab?ticketID={0}&interactionID={1}&chatData={2}', ticketID, interactionID, jsonData);
    var chatID = String.format("{0}_{1}", interactionID, ticketID);
    var tabs = $(".tabhead"), contents = $(".tabcontent");
    var index = tabs[0].children.length + 1;
    var templateTab = "<dd id=\"dd{0}\" class=\"tab tab_t1 {1}\" onclick=\"tabboxEvent(this);\">客户"
        + "<div id='{0}' class='tabClose'></div></dd>";
    var templateContent = "<div id=\"div{0}\" class=\"contentitem {1}\"><iframe id=\"contentitem{0}\" class=\"lightframe\" frameborder=\"no\" border=\"0\"></iframe></div>";
    //if (index <= 6) {
    tabs.append(String.format(templateTab, chatID, (index == 1) ? "selected" : "", interactionID.substr(interactionID.length - 4, 4)));
    contents.append(String.format(templateContent, chatID, (index == 1) ? "selected" : ""));
    $("#contentitem" + chatID).attr("src", viewUrl);
    tabboxEventAll();
    //$(window).focus(100);
    //}
}

//chat结束
function eventRevoked(chatID) {
    // 关闭chat
    var win = top.window;
    var tabs = win.$(".tabhead"), contents = win.$(".tabcontent");
    tabs.children("dd").remove(String.format("[id='dd{0}']", chatID));
    contents.children("div").remove(String.format("[id='div{0}']", chatID));
    if (tabs.length > 0) {
        if (tabs[0].children.length == 0) win.closeWindow();
        else
            win.tabboxEventAll();
    }
}

function closeTabWindow(chatID) {
    var rv = confirm("确认要关闭窗口吗？");
    if (rv) {
        lenovoTrace('chat|' + chatID + '|unload');
        var $content = $("#contentitem" + chatID);
        if ($content.length > 0) {
            var win = $content[0].contentWindow;
            if (win && win.chat) {
                LogMessage('chat.js(141) leftChat');
                win.chat.leftChat();
            }
            else
                eventRevoked(chatID);
        }
    }
    return rv;
}

// 异步请求
function sendRequest(url, params, callback, isAsync,chatID) {
    $.ajax({
        url: url,
        data: params,
        dataType: "jsonp",
        jsonp: 'jsoncallback',
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //var cl = window.console;
            //if (cl) {
            //    cl.log(e.error);
            //    cl.log(e.statusText);
            //}
            lenovoTrace('chat| chatID: ' + chatID + '|sendRequest');
            lenovoTrace('chat| url: ' + url + '|sendRequest');
            lenovoTrace('chat| error1: ' + XMLHttpRequest.status + '|sendRequest');
            lenovoTrace('chat| error1: ' + XMLHttpRequest.readyState + '|sendRequest');
            lenovoTrace('chat| error1: ' + XMLHttpRequest.responseText + '|sendRequest');
            lenovoTrace('chat| error2: ' + textStatus + '|sendRequest');
            lenovoTrace('chat| error3: ' + errorThrown + '|sendRequest');
        },
        async: isAsync == undefined ? true : isAsync,
        success: function (data) {
            callback(data);
        }
    });
}



var XMLHTTPVERSIONCACHE = '';
function sendXMLRequest(url, data, success, async, error) {
    /// <summary>
    /// 通过Msxml2.XMLHTTP请求
    /// </summary>
    /// <param name="url">链接</param>
    /// <param name="data">数据json格式</param>
    /// <param name="success">请求成功事件</param>
    /// <param name="async">true为异步 false同步</param>
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
        try{
            xmlHttp.open("POST", url, async);
        } catch (e) {
            if (error != undefined) {
                (error)(e);
            }
            return;
        }
        xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == '4') {
                if (xmlHttp.status == 200) {
                    if (success != undefined) {
                        (success)(xmlHttp.responseText);
                    }
                    else {
                        (error)("undefined");
                    }
                }
                else {
                    if (error != undefined) {
                        (error)("status not 200");
                    }
                }
            }
            else {
                if (error != undefined) {
                    (error)("readyState not 4");
                }
            }
        }    
        xmlHttp.send($.param(data));
    }
}

// 加载客服组列表
function loadSkills(isTransfer) {
    var url = String.format('{0}/CFG/GetMMSkills?ss={1}&jsoncallback?', domain_phone, new Date().getTime());
    var params = { skill: $("#txtSkillName").val() };
    sendRequest(url, params, function (data) {
        if (data) {
            var $skills = $("#skills table tbody");
            $skills.html("");
            for (var ii = 0; ii <= data.length - 1; ii++) {
                var item = data[ii];
                if (isTransfer) {
                    var btnTsansfer = String.format("<input class=\"transfer icon_btn2\" type=\"button\" value=\"转出\" title=\"{0}\" />", item.Name);
                    var row = String.format("<tr><td><a href='javascript:;' title='{0}'>{0}</a></td><td width='86px'>{1}</td></tr>", item.Name, btnTsansfer);
                }
                else {
                    var row = String.format("<tr><td><a href='javascript:;' title='{0}'>{0}</a></td></tr>", item.Name);
                }
                $skills.append(row);
            }

            // 转接客服组
            $(".transfer", $skills).click(function () {
                var obj = { typeName: 'Skill', queueName: this.title };
                window.returnValue = obj;
                window.close();
            });
            // 查看客服
            $("a", $skills).click(function () {
                $("#hdSkillName").html(this.title);
                loadAgents(isTransfer);
            });
        }
    });
}

// 加载客服列表
function loadAgents(isTransfer) {
    var url = String.format('{0}/ChatStat/GetAgents?ss={1}&jsoncallback?', domain_phone, new Date().getTime());
    var params = { skillName: $("#hdSkillName").text(), agentName: $("#txtAgentName").val(), agentID: '', placeID: '' };
    sendRequest(url, params, function (data) {
        if (data) {
            var $agents = $("#agents table tbody");
            $agents.html("");
            var employeeID = ''
            var reg = new RegExp("(^|&)employeeID=([^&]*)(&|$)", "i");
            var result = window.location.search.substr(1).match(reg);
            if (result != null) employeeID = result[2];
            for (var ii = 0; ii <= data.length - 1; ii++) {
                var item = data[ii];
                if (item.AgentId == employeeID) continue;
                var row = '';
                if (!item.IsReady)
                    row = String.format("<tr style='background-color:red;'><td>{0}</td><td>未就绪</td><td width='86px'></td></tr>", item.AgentId);
                else {
                    var btnTsansfer = String.format("<input class=\"transfer icon_btn2\" type=\"button\" value=\"{0}\" aid=\"{1}\" pid=\"{2}\"  />"
                        , isTransfer ? '转出' : '邀请', item.AgentId, item.PlaceId);
                    row = String.format("<tr><td>{0}</td><td>就绪</td><td width='86px'>{2}</td></tr>"
                       , item.AgentId, '', btnTsansfer);
                }
                $agents.append(row);
            }

            // 转接客服
            $(".transfer", $agents).click(function () {
                var aid = $(this).attr("aid"), pid = $(this).attr("pid");
                var obj = { typeName: 'Agent', agentID: aid, placeID: pid, queueName: $("#hdSkillName").html() };
                window.returnValue = obj;
                window.close();
            });


        }
    });
}

// 加载历史聊天记录
function loadChatHistoryNav() {
    var url = String.format('{0}Db/GetChatHistory?ss={1}&jsoncallback?', window.top.domain_phone_Db, new Date().getTime());
    var params = { employeeID: $("#EmployeeID").val(), machineNO: $("#txtMachineNO").val(), customerID: $("#txtCustomerID").val(), beginTime: $("#txtBeginTime").val(), endTime: $("#txtEndTime").val() };
    sendRequest(url, params, function (data) {
        if (data) {
            var $chats = $("#chatList table tbody");
            $chats.html("");
            for (var ii = 0; ii <= data.length - 1; ii++) {
                var item = data[ii];
                if (!item.CustomerID || item.CustomerID == "") item.CustomerID = '&nbsp;';
                if (!item.MachineNo || item.MachineNo == "") item.MachineNo = '&nbsp;';


                var btnView = String.format("<input class='icon_btn2' type='button' value='查看'"
                    + "title='{0}' onclick=\"loadChatHistory(\'{0}\');\" />", item.ChatID);
                var btnDownload = String.format("<input class='icon_btn2' type='button' value='下载'"
                    + "title='{0}' onclick=\"loadChatHistory(\'{0}\',1);\" />", item.ChatID);
                var row = String.format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}&nbsp;{8}</td></tr>"
                    , item.ChatID, item.EmployeeID, item.CustomerID, item.CustomerName, item.MachineNo
                    , convertDate(item.ChatBeginTime), convertDate(item.ChatEndTime)
                    , btnView, btnDownload);
                $chats.append(row);
            }
            function convertDate(strDate) {
                var theTime = strDate.replace(/\/Date\(/ig, '').replace(/\)\//ig, '').replace(/[A-Za-z]*/ig, '');
                return new Date(parseInt(theTime)).format('yyyy-MM-dd HH:mm:ss');
            }
        }
    });
}

// 加载历史聊天记录
function loadChatHistory(id, isSave) {
    var url = String.format('{0}Db/GetChatHistoryByID?ss={1}&jsoncallback?', window.top.domain_phone_Db, new Date().getTime());
    var params = { chatID: id };
    sendRequest(url, params, function (data) {
        if (data) {
            var $p = $("#pArea");
            $p.html('');
            for (var i = 0; i < data.length; i++) {
                var msg = unescape(data[i]);
                $p.append(msg);
                $p[0].scrollTop = $p[0].scrollHeight;
            }
            if (isSave) {
                var domainSkip = "javascript:void((function(){var d=document;d.open();d.domain='" + document.domain + "';d.charset = \"UTF-8\";d.write('');d.close()})())";
                if (document.domain == document.location.hostname) domainSkip = 'javascript:;';
                $(document.body).after('<iframe src="' + domainSkip + '" id="downWin" style="width:0;height:0"></iframe>');

                try {
                    docInit();
                } catch (e) {
                    setTimeout(function () {
                        docInit();
                    }, 30);
                }
                function docInit() {
                    var win = $('#downWin')[0].contentWindow;
                    var doc = win.document;
                    doc.write($p.html());
                    try {
                        doc.charset = 'UTF-8';
                    }
                    catch (e) { }
                    doc.execCommand("SaveAs", false, id + ".htm");
                }
            }// end if

        }
    });
}

var chatTab = function () {
    var _this = this, _agent = window.top.getAgentInfo(), chs = window.top.domain, isDisposeBusyness = false, isChatOver = false, chatBeginDate = '', WinSR = null, chatEndDate = '';
    var timerHandle = null, timerHandle2 = null, timerHandle3 = null;
    var second = 0, minute = 0, hour = 0, pauseSeconds = 0;
    var arrStatus = { connected: 0, chatting: 1, closed: 2 }
    var currentStatus = arrStatus.connected, isTransfer = false;
    var ticketID = $("#TicketID").val(), sessionID = $("#CurrentSessionID").val(), $panel = $("#divPanel");
    var strChatData = $("#ChatData").val(), chatData = eval('(' + strChatData + ')'), bizData = null;
    var sysTitle = "<span class='Alert'><b>系统 $time$</b></span><br /> ";
    $(document).mousedown(function () { $('#divShadow').hide(); $panel.hide(); });
    $panel.mousedown(function (ev) { ev.stopPropagation() });

    this.debugInfo = function () {
        LogMessage($.param({
            chatEndDate: chatEndDate
        }));
    }

    this.getID = function () {
        return sessionID;
    }

    // 获取聊天消息
    this.showMsg = function (data) {
        if (!data || !data.MsgList) return;

        if (isChatOver) {
            return;
        }

        var $p = $("#pArea"), $t = $("#pType");
        for (var i = 0; i < data.MsgList.length; i++) {
            var msg = data.MsgList[i];
            $p.append(msg);
            $p[0].scrollTop = $p[0].scrollHeight;
            _this.flickTab();
        }
        $t.html((data.IsTyping == 1) ? "用户正在输入中……" : "　");
        if (data.Status == -1) {
            clearTimeout(timerHandle);
            clearTimeout(timerHandle2);
            _this.stopChat();

            if (currentStatus != arrStatus.closed) {
                // 如果不是转接，关闭聊天室
                if (!isTransfer)
                    window.top.stopProcessiong(sessionID);
                //用户已经离开
                $p.append(sysTitle.replace('$time$', new Date().format("HH:mm:ss")));
                $p.append("<div class='MsgContent'><font color='red'>用户已离开，聊天结束。请您关闭窗口。</font></div>");

                $p[0].scrollTop = $p[0].scrollHeight;

                isChatOver = true;
                $t.html('　');

                if (chatEndDate == '') {
                    var url = String.format('{0}/Chat/GetNow?ss={1}&jsoncallback?', chs, new Date().getTime());
                    sendXMLRequest(url, {}, function (data) {
                        chatEndDate = new String(data).replace(/"/g, '');
                        LogMessage('showMsg-chatEndDate:' + chatEndDate);
                    }, false);
                }

                //chatend
                if (chatData.ChatID) {
                    SPhone_Chat.ChatEnd(chatData.ChatID);
                }
            }
            currentStatus = arrStatus.closed;
        }
    }

    this.connectionServer = function () {
        var preQ = '';
        if (chatData.FromQueue != undefined) {
            preQ = chatData.FromQueue;
        }

        var content = String.format('随路数据：当前队列 {0}', chatData.CurrentQueue.split('_')[0]);
        if (preQ != '') {
            content += ' / 转入队列 ' + preQ.split('_')[0];
        }
        $("#spSuilu").html(content);

        var url = String.format('{0}/Chat/ConnectionServer?ss={1}&jsoncallback?', chs, new Date().getTime());
        var params = {
            tid: ticketID, interactionID: sessionID, queueName: chatData.CurrentQueue
            , agentID: _agent.AgentID, placeID: _agent.PlaceID, nickName: _agent.FirstName
            , host: chatData.ChatServerHost, port: chatData.ChatServerPort, isMeeting: chatData.IsConference
        };
        lenovoTrace('chat|' + sessionID + ' |AgentID:' + _agent.AgentID + ' |ConnectionServer', true);
        sendRequestConnection(url, params, function (data) {
            if (data) {
                lenovoTrace('chat|data.Code: ' + data.Code + ' |ConnectionServer', true);
                if (data.Code != 1)
                    timerHandle = setTimeout(_this.connectionServer, 3000);
                else {
                    LogMessage("=======Connect Server 386======");
                    clearTimeout(timerHandle);
                    currentStatus = arrStatus.connected;

                    chatData.SessionID = sessionID;
                    chatData.EmployeeID = _agent.AgentID;
                    chatData.ChatID = data.d.ChatID;
                    chatData.ChatBeginTime = data.d.ChatBeginTime;

                    lenovoTrace('chat|' + sessionID + ' |AgentID:' + _agent.AgentID + ' |NewChat', true);
                    // 新增聊天记录
                    window.top.SPhone_Chat.NewChat(chatData);
                    _this.getChatDuration();

                    chatBeginDate = data.d.StrChatBeginTime;

                    //通知业务系统
                    ChatBizData.ToLenovoBusiness(1, {
                        ChatID: chatData.ChatID,
                        ChatBeginTime: chatBeginDate,
                        EmployeeID: chatData.EmployeeID,
                        InQuene: chatData.FromQueue,
                        CurrentQuene: chatData.CurrentQueue,
                        OutQuene: '',
                        CustomerName: chatData.CustomerName,
                        CustomerID: chatData.CustomerID,
                        MachineNo: chatData.MachineNo,
                        strServiceCardNo: chatData.CardNo
                    });
                }
            }
            else {
                lenovoTrace('chat|' + sessionID + ' |else', true);
            }
        });
    }

    this.sendMessage = function (msg) {
        msg = msg.replace(/<\/?(script)[^>]*>/ig, "");
        if (currentStatus != arrStatus.connected) {
            alert('当前对话不可用，请检查是否连接到服务器。');
            return;
        }
        var url = String.format('{0}/Chat/SendMessage?ss={1}&jsoncallback?', chs, new Date().getTime());
        var params = { message: msg, tid: ticketID, interactionID: sessionID, agentID: _agent.AgentID };
        if (msg == null) return;
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

    this.releaseParty = function (isKeepRoomAlive) {
        var url = String.format('{0}/Chat/LeftChat?ss={1}&jsoncallback?', chs, new Date().getTime());
        var params = { tid: ticketID, interactionID: sessionID, agentID: _agent.AgentID, queueName: chatData.NextQueue, keepAlive: isKeepRoomAlive ? 1 : 0 };

        //alert(chatData.CustomerIP);
        //alert($("#spCustomerIP").text());
        chatData.CustomerLocation = $("#spCustomerIP").text();

        var textContent = '-';
        LogMessage('chat.js(423)releaseParty begin');
        sendXMLRequest(url, params, function (data) { textContent = data; }, false);
        LogMessage('=========chat.js(426)=============');
        chatData.ContentText = textContent;

        chatData.ChatEndTime = new Date().getTime();
        var cstime = new Date().format('yyyy-MM-dd HH:mm:ss').replace(/[- :]?/ig, '');
        if (bizData) {
            bizData.ChatEndTime = cstime;
        }
        try {
            // 更新聊天记录
            if (chatEndDate == '') {
                var url = String.format('{0}/Chat/GetNow?ss={1}&jsoncallback?', chs, new Date().getTime());
                sendXMLRequest(url, {}, function (data) {
                    chatEndDate = new String(data).replace(/"/g, '');
                    LogMessage('releaseParty-chatEndDate:' + chatEndDate);
                }, false);
            }
            chatData.chatEndDate = chatEndDate;
            SPhone_Chat.UpdateChat(chatData);
        }
        catch (e) { }
        try {
            //通知业务系统
            ChatBizData.ToLenovoBusiness(2, {
                ChatID: chatData.ChatID,
                OutQuene: chatData.NextQueue,
                ChatEndTime: cstime
            });
        }
        catch (e) { }

        // 强制关闭时，关闭选项卡
        if (!isKeepRoomAlive) {
            var cid = String.format("{0}_{1}", sessionID, ticketID);
            eventRevoked(cid);
        }
    }

    this.leftChat = function (isKeepRoomAlive) {
        if (!isTransfer) {
            try {
                LogMessage("IsConference:" + chatData.IsConference);
                if (chatData.IsConference == 1)
                    window.top.leaveRoom(sessionID);
                // 当强制关闭，且不是转接时，停止执行
                if (!isKeepRoomAlive)
                    top.window.stopProcessiong(sessionID);
            }
            catch (e) { }
        }

        if (chatEndDate == '') {
            var url = String.format('{0}/Chat/GetNow?ss={1}&jsoncallback?', chs, new Date().getTime());
            sendXMLRequest(url, {}, function (data) {
                chatEndDate = new String(data).replace(/"/g, '');
                LogMessage('leftChat-chatEndDate:' + chatEndDate);
            }, false);
        }

        _this.releaseParty(isKeepRoomAlive);
    }

    this.transferChat = function () {
        var url = String.format('/Chat/Transfer?employeeID={0}&ss={1}', _agent.AgentID, new Date().getTime());
        var returnValue = window.top.showDialog(url, 700, 500, true);
        // 转队列
        if (returnValue && returnValue != undefined) {
            var obj = returnValue;
            isTransfer = true;
            _this.sendMessage('[' + _agent.AgentID + '] 将用户转接至其他工程师。');
            switch (returnValue.typeName) {
                case 'Skill':
                    chatData.NextQueue = obj.queueName;
                    window.top.transferChatQueue(sessionID, chatData.NextQueue, chatData.CurrentQueue, chatData.ChatID);
                    break;
                case 'Agent':
                    window.top.transferChatPerson(sessionID, obj.agentID, obj.placeID, chatData.ChatID, obj.queueName);
                    break;
            }
            _this.leftChat(true);
        }
    }

    this.meetingInvite = function () {
        var url = String.format('/Chat/Meeting?employeeID={0}&ss={1}', _agent.AgentID, new Date().getTime());
        var returnValue = window.top.showDialog(url, 700, 500, true);
        // 多方通话
        if (returnValue && returnValue != undefined) {
            var obj = returnValue;
            chatData.IsConference = 1;
            window.top.chatMeeting(sessionID, obj.agentID, obj.placeID);
        }
    }

    this.setRTO = function () {
        chatData.IsRTO = 1;
        var accessCode = String.format("{0}_{1}", _agent.AgentID, new Date().getTime());
        var url = String.format('/Chat/ChatRTO?ticketID={0}&interactionID={1}&accessCode={2}', ticketID, sessionID, accessCode);
        var feature = 'width=200px,height=100px,top=0,left=0,toolbar=no,menubar=no,scrollbars=no,resizable=no,location=no,status=no';
        window.top.open(url, 'rto', feature);

        //rto协议
        var msg = "<font color=red>您好，联想工程师将对您进行远程协助，请认真阅读以下服务协议：<br/>"
            + "1、客户了解并同意其有责任自行对机器中所有程序、数据、资料备份并确保其安全;<br/>"
            + "2、客户承诺其对相关软件拥有合法使用权，授权联想为该等软件提供服务将不会损害任何版权人、或其它经版权人许可使用人的合法权益;<br/>"
            + "3、客户承诺并保证该被提供服务机器不存在任何阻碍服务进行的法律责任或限制;<br/> "
            + "4、客户授权工程师通过网络远程接管您的机器，并在您的监督许可下完成整个远程操作;<br/> "
            + "5、客户了解并知悉联想仅对客户拥有合法使用权的软件提供服务支持;<br/> "
            + "6、按Ctrl + Z ，您可以随时退出远程接管状态;<br/> "
            + "7、选择“同意并开始服务”，即表明您已经阅读并同意本服务协议.<br/> </font>";
        if (chatData.CurrentQueue == "Q811_CrashRecure")
            msg += 'multimedia.lenovo.com.cn:443:' + accessCode;
        else
            msg += String.format('如果允许客服的远程协助请求，请点击 <a href="/Chat/ChatRTO?accessCode={0}" target="_blank">同意</a>'
                , accessCode);
        // 发送消息
        _this.sendMessage("$$RTO$$" + encodeURIComponent(msg));
    }

    this.viewHistory = function () {
        if (isemergency) {
            var url = String.format('/Chat/ChatHistory?employeeID={0}&ss={1}', _agent.AgentID, new Date().getTime());
            var feature = 'width=850,height=600,top=0,left=0,toolbar=no,menubar=no,scrollbars=no,resizable=no,location=no,status=no';
            window.open(url, 'historyWindow', feature);
        }
        else {
            var CustomerID = chatData.CustomerID;
            var MachineNo = chatData.MachineNo;
            var serviceCardNo = chatData.CardNo;
            var url = String.format(Business_AppUrl + 'IR/IRSearch.aspx?type=2&CustomerID={0}&MachineNo={1}&strServiceCardNo={2}', CustomerID, MachineNo, serviceCardNo);
            var feature = 'width=1105,height=600,toolbar=no,menubar=no,scrollbars=no,resizable=yes,location=no,status=no';
            window.open(url, 'historyWindow', feature);
        }
    }

    this.showLinks = function (ev) {
        var $content = $('<div class="eBox"><h3>请点击链接（将在新窗口打开）</h3><div class="eList"></div></div>');
        var jList = $('.eList', $content), items = [];
        if (links) {
            jList.html("");
            $(links).each(function () {
                items.push(String.format('<div><a href="{0}" target="_blank">{0}</a></div>', this));
            });
            jList.append(items.join(''));
        }
        showPanel(ev, $content.html());
    }

    this.createSR = function () {
        bizData = window.top.ChatBizData;
        bizData.ChatID = chatData.ChatID;
        bizData.CustomerID = chatData.CustomerID;
        bizData.FromQueue = chatData.FromQueue;
        bizData.CurrentQueue = chatData.CurrentQueue;
        bizData.NextQueue = chatData.NextQueue;
        bizData.CardNo = chatData.CardNo;
        bizData.RegNo = chatData.RegNo;
        bizData.SessionID = chatData.SessionID;
        bizData.ChatBeginTime = chatBeginDate;
        bizData.PreChatID = chatData.PreChatID;
        bizData.CustomerName = $('#CustomerName').html();
        bizData.MachineNo = $('#MachineNo').html();
        bizData.IsSRCreated = true;
        bizData.email = chatData.MailAddress;
        if (!WinSR || WinSR.closed) {
            WinSR = top.window.ChatBizData.CreateSR(bizData);
        }
        else {
            try {
                WinSR.focus();
            }
            catch (e) { }
        }
    }

    function showPanel(ev, content) {
        $panel.html('').append(content).css('left', -999).css('top', -999);
        var btnXY = $(ev).offset(), btnWidth = $(ev).outerWidth(), btnHeight = $(ev).outerHeight(), panelWidth = $panel.outerWidth(), panelHeight = $panel.outerHeight();
        var x = btnXY.left, y = btnXY.top + btnHeight, maxX = 500;
        if ((x + panelWidth) > maxX) x -= (panelWidth + btnWidth);
        var shadowBorder = 3;
        $("#divShadow").css({ 'left': x + shadowBorder, 'top': y + shadowBorder, 'width': panelWidth, 'height': panelHeight }).show();
        $panel.css({ 'left': x, 'top': y }).show();
    }

    // 闪动tab
    this.flickTab = function () {
        var selector = String.format("dd[id='dd{0}_{1}']", sessionID, ticketID);
        var tab = $(".tabhead", window.top.document).children(selector);
        if (tab) {
            clearTimeout(timerHandle3);
            if (tab.hasClass("selected")) {
                tab.removeClass('flick');
                tab.removeClass('alert');
                pauseSeconds = 0;
            }
            else {
                if (currentStatus == arrStatus.connected) pauseSeconds++;
                if (pauseSeconds >= 35) {
                    if (tab.hasClass('flick')) tab.removeClass('flick');
                    if (!tab.hasClass('alert')) tab.addClass('alert');
                }
                else {
                    if (tab.hasClass('alert')) tab.removeClass('alert');
                    if (!tab.hasClass('flick')) tab.addClass('flick');
                }
                timerHandle3 = setTimeout(_this.flickTab, 1000);
            }
        }
        if (!top.hasFlickWin)
            top.needFlick = true;
    }
    this.stopChat = function () {
        // 停止闪动
        top.needFlick = false;
        clearTimeout(timerHandle3);

        var selector = String.format("dd[id='dd{0}_{1}']", sessionID, ticketID);
        var tab = $(".tabhead", window.top.document).children(selector);
        if (tab) {
            tab.removeClass('flick');
            tab.removeClass('alert');
            tab.addClass('stop');
        }
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
    //end

    // 异步请求
    sendRequestConnection = function (url, params, callback, isAsync) {
        $.ajax({
            url: url,
            data: params,
            dataType: "jsonp",
            jsonp: 'jsoncallback',
            error: function (XMLHttpRequest, textStatus, errorThrown) {

                lenovoTrace('chat| error1: ' + XMLHttpRequest.status + '|sendRequestConnection');
                lenovoTrace('chat| error1: ' + XMLHttpRequest.readyState + '|sendRequestConnection');
                lenovoTrace('chat| error1: ' + XMLHttpRequest.responseText + '|sendRequestConnection');
                lenovoTrace('chat| error2: ' + textStatus + '|sendRequestConnection');
                lenovoTrace('chat| error3: ' + errorThrown + '|sendRequestConnection');

                var myDate = new Date();
                var date = makeStamp(myDate);

                chatData.SessionID = params.interactionID;
                chatData.EmployeeID = params.agentID;
                chatData.ChatID = String.format("{0}{1}C", params.agentID, date);
                //chatData.ChatBeginTime = new Date().getTime();

                lenovoTrace('chat| SessionID' + chatData.SessionID + '| EmployeeID' + chatData.EmployeeID + '| ChatID' + chatData.ChatID + ' |NewChat', true);

                // 新增聊天记录
                window.top.SPhone_Chat.NewChat(chatData);

                _this.getChatDuration();

                chatBeginDate = makeStampStr(myDate);

                lenovoTrace('chat| ChatID：' + chatData.ChatID + '| ChatBeginTime：' + chatBeginDate + '| EmployeeID：' + chatData.EmployeeID +
                    '| InQuene：' + chatData.FromQueue + '| CurrentQuene：' + chatData.CurrentQueue + '| CustomerName：' + chatData.CustomerName +
                     '| CustomerID：' + chatData.CustomerID + '| MachineNo：' + chatData.MachineNo + '| strServiceCardNo：' + chatData.CardNo + ' |ChatBizData.ToLenovoBusiness', true);

                //通知业务系统
                ChatBizData.ToLenovoBusiness(1, {
                    ChatID: chatData.ChatID,
                    ChatBeginTime: chatBeginDate,
                    EmployeeID: chatData.EmployeeID,
                    InQuene: chatData.FromQueue,
                    CurrentQuene: chatData.CurrentQueue,
                    OutQuene: '',
                    CustomerName: chatData.CustomerName,
                    CustomerID: chatData.CustomerID,
                    MachineNo: chatData.MachineNo,
                    strServiceCardNo: chatData.CardNo
                });

            },
            async: isAsync == undefined ? true : isAsync,
            success: function (data) {
                callback(data);
            }
        });
    }

    makeStamp = function(d) {
        debugger;
        var y = d.getFullYear(),
            M = d.getMonth() + 1,
            D = d.getDate(),
            h = d.getHours(),
            m = d.getMinutes(),
            s = d.getSeconds(),
            ms = d.getMilliseconds(),
            pad = function (x) {
                x = x + '';
                if (x.length === 1) {
                    return '0' + x;
                }
                return x;
            };
        return y + pad(M) + pad(D) + pad(h) + pad(m) + pad(s) + pad(ms);
    }

    makeStampStr = function (d) {
        debugger;
        var y = d.getFullYear(),
            M = d.getMonth() + 1,
            D = d.getDate(),
            h = d.getHours(),
            m = d.getMinutes(),
            s = d.getSeconds(),
            pad = function (x) {
                x = x + '';
                if (x.length === 1) {
                    return '0' + x;
                }
                return x;
            };
        return y + '-' + pad(M) + '-' + pad(D) + ' ' + pad(h) + ':' + pad(m) + ':' + pad(s);
    }

}

//true异步
function lenovoTrace(msg, async) {
    if (async == undefined) {
        async = false;
    }
    try {
        sendXMLRequest('/LenovoTrace.aspx', { act: 'trace', msg: msg }, function () { }, async);
    }
    catch (e) { }
}