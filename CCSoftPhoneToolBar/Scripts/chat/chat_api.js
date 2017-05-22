/// <reference path="../softphone_api.js" />
/// <reference path="chat.js" />
/// <reference path="../lenovo_business_cti.js" />

//0.记录日志
function LogMessage(msg) {
    try {
        window.opener.LogMessage(msg);
    }
    catch (e) {
        try {
            top.opener.LogMessage(msg);
        }
        catch (e2) {
            try {
                if (console) {
                    console.info(msg);
                }
            } catch (e) { }
        }
    }
}

//1.离开聊天室
function leaveRoom(interactionId) {
    try {
        window.opener.SoftPhone.LeaveInteraction_ISvr(interactionId);
    }
    catch (e) {
        if (console) console.log(e.message);
    }
}

//2.关闭Interaction连接
function stopProcessiong(interactionId) {
    window.opener.SoftPhone.StopProcessing_ISvr(interactionId);
}

//3.转队列
function transferChatQueue(interactionId, queueName, currentQueueName, chatID) {
    // 更改随路数据
    setChatData(interactionId, [
        { Key: "IsTransfer", Value: "1" },
        { Key: "TargetSkill", Value: queueName },
        { Key: "strPreCallIDChat", Value: chatID },
        { Key: "strPreAgentgroupNameChat", Value: currentQueueName }
    ]);
    window.opener.SoftPhone.PlaceInQueue_ISvr(interactionId, 'ChatBJ Transfer Queue');
}

//3.转个人
function transferChatPerson(interactionId, agentId, placeId, chatID, queueName) {
    // 更改随路数据
    setChatData(interactionId, [
        { Key: "strPreCallIDChat", Value: chatID },
        { Key: "TargetSkill", Value: queueName }
    ]);
    // 转人
    window.opener.SoftPhone.Transfer_ISvr(interactionId, agentId, placeId);
}

//4.会议（只能到个人）
function chatMeeting(interactionId, agentId, placeId) {
    setChatData(interactionId, [{ Key: "IsConference", Value: "1" }]);
    window.opener.SoftPhone.Conference_ISvr(interactionId, agentId, placeId);
}


//6.修改随路数据
function setChatData(interactionId, userDataListKeyValueJson) {
    var userDataListKeyValueJson = JSON.stringify(userDataListKeyValueJson);
    window.opener.SoftPhone.ChangeProperties_ISvr(interactionId, userDataListKeyValueJson);
}

//7.


//8.获取agentID[员工编号],placeID[坐席编号],firstName[坐席名称]
function getAgentInfo() {
    var agentID = window.opener.cfg.IServerConfig.AgentID;
    var placeID = window.opener.cfg.IServerConfig.PlaceID;
    var firstName = window.opener.$('#FirstName').text();
    return { AgentID: agentID, PlaceID: placeID, FirstName: firstName };
}


/************************修改随路数据*************************/

function afterEnterChat(interactionId, chatID) {
    // 进入之后，更改随路数据
    setChatData(interactionId, [
        { Key: "strCurCallIDChat", Value: chatID },
    ]);
}

function beforeLeaveChat(chatData) {
    // 离开之前，更改随路数据
    setChatData(chatData.SessionID, [
        { Key: "strPreCallIDChat", Value: chatData.ChatID },
        { Key: "strPreAgentgroupNameChat", Value: chatData.NextQueue }
    ]);
}


/************************修改随路数据*******************************/

/************************提交业务***********************************/
var ChatBizData = {
    CustomerID: '',
    ChatID: '',
    RegNo: '',
    SessionID: '',
    CurrentQueue: '',
    FromQueue: '',
    CardNo: '',
    PreChatID: '',
    NextQueue: '',
    ChatBeginTime: '',
    ChatEndTime: '',
    // 是否已创建SR
    IsSRCreated: false,
    WinSR: '',
    CustomerName: '',
    MachineNo: '',
    email: '',

    CreateSR: function (data) {
        var newData = {};
        newData.CustomerID = data.CustomerID;
        newData.ChatID = data.ChatID;
        newData.RegNo = data.RegNo;
        newData.SessionID = data.SessionID;
        newData.CurrentQueue = data.CurrentQueue;
        newData.FromQueue = data.FromQueue;
        newData.CardNo = data.CardNo;
        newData.NextQueue = data.NextQueue;
        newData.ChatBeginTime = data.ChatBeginTime;
        newData.IRIDType = 2;
        newData.CustomerName = data.CustomerName;
        newData.HideRecord = 'false';
        newData.MachineNo = data.MachineNo;
        newData.email = data.email;

        var url = Business_AppUrl + "index.aspx?" + $.param(newData);
        //if (!WinSR || WinSR.closed) {
        return window.open(url + String.format("&ss=", new Date().getTime()), 'sr' + data.ChatID, 'height=800,width=1440,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no,left=0,top=0');
        //}
    },


    //通知业务系统:1开始 2结束
    ToLenovoBusiness: function (type, data) {
        if (!isemergency) {
            try {
                top.opener.top.ChatCallFunc(type, JSON.stringify(data));
            }
            catch (e) { }
        }
    }
}


/************************提交业务***********************************/


/************************修改聊天记录*******************************/

var SPhone_Chat = {
    CustomerID: '',
    CustomerName: '',
    MachineNo: '',
    MailAddress: '',
    CardNo: 0,
    RegNo: 0,
    WSISID: '',

    ChatID: '',
    EnterID: '',
    EmployeeID: '',
    PlaceIP: '',
    ChatBeginTime: '',
    ChatEndTime: '',
    SessionID: '',
    CurrentQueue: '',
    FromQueue: '',
    NextQueue: '',
    ContentText: '',
    IsConference: 0,
    IsTransfer: 0,
    IsRTO: 0,
    // 上一个ChatID
    PreChatID: '',

    ChatServerHost: '',
    ChatServerPort: '',
    CustomerIP: '',
    CustomerLocation: '',

    Init: function (data) {
        this.EnterID = gv('EnterID');
        this.CustomerID = gv('UserID');
        this.CustomerName = gv('UserName');
        this.MachineNo = gv('MachineNo');
        this.MailAddress = gv('emailClient');
        this.CurrentQueue = gv('TargetSkill');
        this.FromQueue = gv('strPreAgentgroupNameChat');
        //this.NextQueue = gv('strNextAgentgroupNameChat');
        this.CardNo = gv('strServiceCardNo');
        this.RegNo = gv('RegisterNumber');
        this.WSISID = gv('WSISID');

        this.IsConference = gv('IsConference') == '' ? 0 : 1;
        this.IsTransfer = gv('IsTransfer') == '' ? 0 : 1;

        this.PreChatID = gv('strPreCallIDChat');

        this.ChatServerHost = gv('ChatServerHost');
        this.ChatServerPort = gv('ChatServerPort');

        this.CustomerIP = gv('CustomerIP');
        this.CustomerLocation = gv('CustomerLocation');
        
        function gv(key) {
            var value = '';
            $(data).each(function (i) {
                if (key == this.Key)
                    value = this.Value;
            });
            return value;
        }
    },

    
    // 创建新聊天记录
    NewChat: function (chatData) {
        var url = String.format('{0}Db/ChatCreate?ss={1}&jsoncallback?', window.top.domain_phone_Db, new Date().getTime());
        var params = {
            chatID: chatData.ChatID,
            enterID: chatData.EnterID,
            inneractionID: chatData.SessionID,
            fromQueue: chatData.FromQueue,
            currentQueue: chatData.CurrentQueue,
            customerID: chatData.CustomerID,
            customerName: chatData.CustomerName,
            machineNo: chatData.MachineNo,
            mailAddress: chatData.MailAddress,
            cardNo: chatData.CardNo,
            agentID: chatData.EmployeeID,
            beginDate: chatData.ChatBeginTime,
            wSISID: chatData.WSISID,
            isTransfer: chatData.IsTransfer,
            isMeeting: chatData.IsConference
        };
        try {
            LogMessage("=======NewChat begin======");
            sendXMLRequest(url, params, function (data) { }, true, function () {
                setTimeout(function () {
                    SPhone_Chat.NewChat(chatDate);
                }, 3000);
            });
            LogMessage("=======NewChat end======");
        }
        catch (e) {
            if (window.console)
                console.log(e.message);
        }
    },

    // 更新聊天记录
    UpdateChat: function (chatData) {
        var ipLocation = chatData.CustomerLocation;
        ipLocation = ipLocation.replace("所在地区:", "");
        if (ipLocation == "未知") {
            ipLocation = "";
        }

        var url = String.format('{0}Db/ChatUpdate?ss={1}&jsoncallback?', window.top.domain_phone_Db, new Date().getTime());
        var params = {
            chatID: chatData.ChatID,
            nextQueue: chatData.NextQueue,
            isMeeting: chatData.IsConference,
            isRTO: chatData.IsRTO,
            jsonMessageData: chatData.ContentText,
            chatEndDate: chatData.chatEndDate,
            customerIP: chatData.CustomerIP,
            customerLocation: ipLocation
        };
        try {
            LogMessage('更新聊天记录begin.');
            LogMessage('UpdateChat-chatEndDate:' + params.chatEndDate);
            sendXMLRequest(url, params, function (data) { }, false);
            LogMessage('========更新聊天记录succ.=========');
        }
        catch (e) {
            LogMessage('更新聊天记录err:' + e.message);
        }
    },

    // 更新结束时间
    ChatEnd: function (chatID) {

        var url = String.format('{0}Db/ChatEnd?ss={1}&jsoncallback?', window.top.domain_phone_Db, new Date().getTime());
        var params = {
            chatID: chatID
        };
        try {
            LogMessage("=======NewChat begin======");
            sendXMLRequest(url, params, function (data) { });
            LogMessage("=======NewChat end======");
        }
        catch (e) {
            if (window.console)
                console.log(e.message);
        }
    }
};


/************************修改聊天记录*******************************/