/// <reference path="jquery-1.9.1.js" />
/// <reference path="json2.js" />
/// <reference path="softphone_interface.js" />
/// <reference path="softphone_config.js" />
/// <reference path="cti.js" />
/// <reference path="lenovo_business_cti.js" />

/* -------------------------------调试.开始------------------------------- */
$(function () {
    $('.console_clear').click(function () {
        $('.console').html('');
    });
});
function GetNow() {
    return new Date().toLocaleString();
}
function UpdateConsole(type, msg) {
    $('.LENOVODEBUG').append('<li class="' + type + '"><pre><div class="now">' + type + "\t" + GetNow() + "</div>" + msg + '</pre></li>');
    if ($('.LENOVODEBUG_AUTOFLOW').prop('checked')) {
        ConsoleScroll();
    }
}
function ConsoleScroll() {
    $('.LENOVODEBUG').scrollTop($('.LENOVODEBUG').get(0).scrollHeight);
}
function LogMessage(msg) {
    if (LENOVODEBUG) {
        try {
            UpdateConsole('LogMessage', msg);
        }
        catch (e) { }
    }
}
function LogRequest(msg) {
    if (LENOVODEBUG) {
        UpdateConsole('LogRequest', msg);
    }
}
function LogException(msg) {
    if (LENOVODEBUG) {
        UpdateConsole('LogException', msg);
    }
}
function LogResponse(msg) {
    if (LENOVODEBUG) {
        UpdateConsole('LogResponse', msg);
    }
}
/* -------------------------------调试.开始------------------------------- */



//配置
var cfg = __CFG;
//话务流
var vf = __VF;

/*基础begin*/
function switchIcon(area, cmd, title) {
    /// <summary>
    /// 切换图标
    /// </summary>
    /// <param name="area">id不需要#</param>
    /// <param name="cmd">按钮命令</param>
    /// <param name="title">按钮提示</param>
    $('#' + area + ' span:first').attr('class', cmd);
    title = undefined;
    if (title) {
        $('#' + area + ' span:first').attr('title', title);
    }
    else {
        $('#' + area + ' span:first').removeAttr('title');
    }
}
function OcxError(msg, ex, type) {
    /// <summary>
    /// 控件发生错误
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="type">T:Tserver I:IServer</param>
    if (type) {
        if (type == 1) {
            if (ex.indexOf('Genesyslab.Platform.Commons.Protocols.ProtocolException: Connection is not opened') != -1) {
                if (!VOICE_Invalid) {
                    SoftPhone.Connect();
                }
            }
        }
        else if (type == 2) {
            if (ex.indexOf('Genesyslab.Platform.Commons.Protocols.ProtocolException: Connection is not opened') != -1) {
                if (!Chat_Invalid) {
                    LogMessage('Connection is not opened');
                    SoftPhone.Connect_ISvr();
                }
            }
        }
    }
}

function mergeJson(options, jsondata) {
    /// <summary>
    /// 合同json数据
    /// </summary>
    /// <param name="options">默认值</param>
    /// <param name="jsondata">新值</param>
    $.extend(options, jsondata);
}
function CFG_Merge() {
    /// <summary>
    /// 获取ocx配置
    /// </summary>
    if (OCXAutoJsonMerge) {
        mergeJson(cfg, JSON.parse(SoftPhone.GetConfig()));
    }
}
function VF_Merge() {
    /// <summary>
    /// 合并话务流
    /// </summary>
    if (OCXAutoJsonMerge) {
        mergeJson(vf, JSON.parse(SoftPhone.GetVoiceFlow()));
    }
}

function setConnection(lineid, ConnectionID) {
    /// <summary>
    /// 缓存ConnectionID
    /// </summary>
    /// <param name="lineid">0呼入 外拨 1报工号 电话支付</param>
    /// <param name="ConnectionID"></param>
    if (lineid == 0) {
        $('#softphonetoolbar').attr('connectionid', ConnectionID);
    }
    else {
        $('#softphonetoolbar').attr('secondconnectionid', ConnectionID);
    }
}
function getConnection(lineid) {
    /// <summary>
    /// 读取ConnectionID,如果不存在返回undefined
    /// </summary>
    /// <param name="lineid">0线 1线</param>
    /// <returns type=""></returns>
    var result = '';
    if (lineid == 0) {
        result = $('#softphonetoolbar').attr('connectionid');
    }
    else {
        result = $('#softphonetoolbar').attr('secondconnectionid');
    }
    if (result == '') {
        return undefined;
    }
    return result;
}

function setAgentStatusChanged() {
    /// <summary>
    /// 坐席状态变更时显示坐席状态图标
    /// </summary>
    var isEnabled = $('#agentstatus span:first').attr('class') != 'disabled';
    //var isEnabled = getConnection(0) == undefined && getConnection(1) == undefined;

    if ((getConnection(0) == undefined && getConnection(1) == undefined) || VOICE_Invalid) {
        var voice = topJ('#v2_lenovo_voice_onoff').prop('disabled') == false;
        var chat = topJ('#v2_lenovo_chat_onoff').prop('disabled') == false;

        var vs = cfg.TServerConfig.AgentStatus == AgentStatus.就绪;//就绪
        var cs = cfg.IServerConfig.AgentStatus == AgentStatus.就绪;//就绪
        if (VOICE_Invalid) {
            vs = false;
        }
        if (Chat_Invalid) {
            cs = false;
        }
        if (voice && vs && chat && cs) {
            //都就绪
            LogMessage('(164)');
            switchIcon('agentstatus', 'ready', '就绪');
        }
        else if (voice && vs) {
            //voice就绪
            LogMessage('(169)');
            switchIcon('agentstatus', 'voiceready', '语音就绪');
        }
        else if (chat && cs) {
            //chat就绪
            LogMessage('(174)');
            switchIcon('agentstatus', 'chatready', '文本就绪');
        }
        else {
            //都未就绪
            LogMessage('(179)');
            switchIcon('agentstatus', 'notready', '未就绪');
        }
    }
}
function setMenuSelected(AgentStateValue) {
    /// <summary>
    /// 选中选择的图标
    /// </summary>
    /// <param name="AgentStateValue">坐席选择的状态:0 3 4 5 6 7 8</param>
    $('#v2_menu li a.current').removeClass('current');
    $('#v2_menu li a[jtag="all:' + AgentStateValue + '"]').addClass('current');
}
/*基础end*/

//软电话API
var SoftPhone = {
    SetConfig: function (options) {
        /// <summary>
        /// [系统]1.配置OCX.只能配置一次
        /// </summary>
        /// <param name="options">SoftPhoneConfig</param>
        SPhone.SetConfig(JSON.stringify(options));
    },
    GetConfig: function () {
        /// <summary>
        /// [系统]获取配置文件
        /// </summary>
        /// <returns type="String"></returns>
        return SPhone.GetConfig();
    },
    GetVoiceFlow: function () {
        /// <summary>
        /// [系统]获取话务流
        /// </summary>
        /// <returns type="String"></returns>
        return SPhone.GetVoiceFlow();
    },
    VoiceFlowUpdate: function (voiceFlowJson) {
        /// <summary>
        /// 更新话务流
        /// </summary>
        /// <param name="voiceFlowJson"></param>
        SPhone.VoiceFlowUpdate(JSON.stringify(voiceFlowJson));
    },
    UpdateUserData: function (connID, userDataListKeyValueJson) {
        /// <summary>
        /// 更新随路数据
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="userDataListKeyValueJson">[{Key:'',Value:''},{Key:'',Value:''}]</param>
        LogRequest(connID);
        LogRequest(JSON.stringify(userDataListKeyValueJson));
        SPhone.UpdateUserData(connID, JSON.stringify(userDataListKeyValueJson));
    },
    DeleteUserData: function (connID) {
        /// <summary>
        /// 清空随路数据
        /// </summary>
        /// <param name="connID"></param>
        SPhone.DeleteUserData(connID);
    },
    SetPreAppExit: function () {
        /// <summary>
        /// 防止无法正常退出,设置后不接收事件.
        /// </summary>
        SPhone.SetPreAppExit();
    },
    Exit: function () {
        /// <summary>
        /// [系统]释放所有资源然后退出
        /// </summary>
        /// <returns type="bool"></returns>
        SoftPhone.SetPreAppExit();//防止无法正常退出,设置后不接收事件.

        var msg = EmployeeID + ',' + AgentIP + ' -> 正在退出(EnableVoice:' + cfg.TServerConfig.EnableVoice + ',IsLinkConnected:' + cfg.TServerConfig.IsLinkConnected + ',IsAgentLogin:' + cfg.TServerConfig.IsAgentLogin + ',IsAgentLogout:' + cfg.TServerConfig.IsAgentLogout + ')';
        var isLogined = false;
        if (cfg.TServerConfig.EnableVoice && cfg.TServerConfig.IsLinkConnected) {
            if (cfg.TServerConfig.AgentStatus == AgentStatus.案头工作 && SF_WaitingDeskTime_CallID != '' && SF_WaitingDeskTime_CallID != null) {
                var data = {
                    CallID: SF_WaitingDeskTime_CallID,
                    EmployeeID: EmployeeID
                };
                try {
                    msg += ' -> 正在设置案面结束';
                    xmlHttpRequest(AppServer_DbUri + 'Db/CallSetDeskTime', data, false, function (r) { });
                } catch (e) { }
                SF_Call0IsEnd = false;
                SF_WaitingDeskTime_CallID = '';
            }

            if (cfg.TServerConfig.IsAgentLogin) {
                isLogined = true;
                msg += ' -> AgentLogout';
                SoftPhone.AgentLogout();
            }
            if (cfg.TServerConfig.IsRegistered && !cfg.TServerConfig.IsUnregistered) {
                msg += ' -> UnregisterAddress';
                SoftPhone.UnregisterAddress();
            }
            msg += ' -> Disconnect';
            SoftPhone.Disconnect();
        }
        if (cfg.IServerConfig.EnableChat && cfg.IServerConfig.IsLinkConnected) {
            msg += ' -> 正在退出Chat';
            if (cfg.IServerConfig.IsAgentLogin && !cfg.IServerConfig.IsAgentLogout) {
                isLogined = true;
                msg += ' -> AgentLogout_ISvr';
                SoftPhone.RemoveMedia_ISvr('chat');
                SoftPhone.AgentLogout_ISvr();
            }
            msg += ' -> Disconnect_ISvr';
            SoftPhone.Disconnect_ISvr();
        }
        var data = {
            personDBID: PERSON_DBID
        };
        try {
            if (isLogined) {
                msg += ' -> 移除DN和Place';
                $.each(NLBOpenStaticAppServers.split(','), function () {
                    xmlHttpRequest(this + 'PhoneStat/AgentLogout', data, false, function (r) { });
                });
            }
        }
        catch (e) { }
        try {
            if (isLogined) {
                lenovoTrace(msg);
            }
        }
        catch (e) { }
        return SPhone.Exit();
    },
    VoiceFlowReset: function () {
        /// <summary>
        /// 重置话务流
        /// </summary>
        SPhone.VoiceFlowReset();
    },
    SetTalking: function (isTalking) {
        /// <summary>
        /// 是否在通话中
        /// </summary>
        /// <param name="isTalking"></param>
        SPhone.SetTalking(isTalking);
    },
    SetDialing: function (isDialing) {
        /// <summary>
        /// 是否正在外拨
        /// </summary>
        /// <param name="isDialing"></param>
        SPhone.SetDialing(isDialing);
    },

    //-----------------------------------------TSVR begin-----------------------------------------
    Connect: function () {
        /// <summary>
        /// 连接
        /// </summary>
        SPhone.Connect();
    },
    Disconnect: function () {
        /// <summary>
        /// 断开连接
        /// </summary>
        SPhone.Disconnect();
    },
    RegisterAddress: function () {
        /// <summary>
        /// 注册分机号
        /// </summary>
        SPhone.RegisterAddress();
    },
    UnregisterAddress: function () {
        /// <summary>
        /// 移除注册分机号
        /// </summary>
        SPhone.UnregisterAddress();
    },
    AgentLogin: function () {
        /// <summary>
        /// 登录
        /// </summary>
        SPhone.AgentLogin();
    },
    AgentLogout: function () {
        /// <summary>
        /// 登出
        /// </summary>
        SPhone.AgentLogout();
    },
    AgentReady: function () {
        /// <summary>
        /// 就绪
        /// </summary>
        SPhone.AgentReady();
    },
    AgentNotReadyDefault: function () {
        /// <summary>
        /// 未就绪
        /// </summary>
        SoftPhone.AgentNotReady('-1', '未就绪', false);
    },
    AgentNotReady: function (reasonCode, reasonName, AfterCall) {
        /// <summary>
        /// 未就绪
        /// </summary>
        /// <param name="reasonCode">理由Code</param>
        /// <param name="reasonName">理由名称</param>
        /// <param name="AfterCall">是否在挂断电话之后</param>
        var lenovoReasonCode = '100';
        switch (reasonCode) {
            case "-1"://未就绪
                lenovoReasonCode = "0";
                break;
            case "1"://处理电话
                lenovoReasonCode = "0";
                break;
            case "2"://案头工作
                lenovoReasonCode = "0";
                break;
            case "3"://后续跟进
                lenovoReasonCode = "8";
                break;
            case "4"://开会
                lenovoReasonCode = "6";
                break;
            case "5"://培训
                lenovoReasonCode = "7";
                break;
            case "6"://休息
                lenovoReasonCode = "4";
                break;
            case "7"://午餐
                lenovoReasonCode = "5";
                break;
            case "8"://其他
                lenovoReasonCode = "100";
                break;
        }
        SPhone.AgentNotReady(reasonCode, reasonName, lenovoReasonCode, AfterCall);
    },
    AnswerCall: function (connID) {
        /// <summary>
        /// 应答
        /// </summary>
        /// <param name="connID"></param>
        SPhone.AnswerCall(connID);
    },
    MakeCall: function (otherDN, userDataListKeyValueJson) {
        /// <summary>
        /// 外拨:CallCenter,ANI,DNIS 已自动附加不需要再加了
        /// </summary>
        /// <param name="otherDN">目的号码</param>
        /// <param name="userDataListKeyValueJson">随路数据</param>
        if (!userDataListKeyValueJson) {
            userDataListKeyValueJson = [];
        }
        if (typeof userDataListKeyValueJson == 'object') {
            userDataListKeyValueJson.push(utils.keyValueToJson('CallCenter', cfg.CallCenter));
            userDataListKeyValueJson.push(utils.keyValueToJson('ANI', cfg.TServerConfig.DN));
            userDataListKeyValueJson.push(utils.keyValueToJson('DNIS', utils.lenovo_getRightDNIS(otherDN)));
        }
        SoftPhone.SetDialing(true);
        switchIcon('answer', 'Dialing', '拨打中');
        SPhone.MakeCall(otherDN, JSON.stringify(userDataListKeyValueJson));
    },
    ReleaseCall: function (connID) {
        /// <summary>
        /// 挂断
        /// </summary>
        /// <param name="connID"></param>
        if (connID && connID != '') {
            SPhone.ReleaseCall(connID);
        }
    },
    HoldCall: function (connID) {
        /// <summary>
        /// 保持通话
        /// </summary>
        /// <param name="connID"></param>
        if (connID && connID != '') {
            SPhone.HoldCall(connID);
        }
    },
    RetrieveCall: function (connID) {
        /// <summary>
        /// 取回通话
        /// </summary>
        /// <param name="connID"></param>
        if (connID && connID != '') {
            SPhone.RetrieveCall(connID);
        }
    },
    MuteTransfer: function (connID, otherDN) {
        /// <summary>
        /// 盲转
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="otherDN"></param>
        SPhone.MuteTransfer(connID, otherDN);
    },
    InitiateTransfer: function (connID, otherDN) {
        /// <summary>
        /// 转接
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="otherDN"></param>
        SPhone.InitiateTransfer(connID, otherDN);
    },
    CompleteTransfer: function (connID, transferConnID) {
        /// <summary>
        /// 确认转接
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="transferConnID"></param>
        SPhone.CompleteTransfer(connID, transferConnID);
    },
    InitiateConference: function (connID, otherDN) {
        /// <summary>
        /// 邀请会议
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="otherDN"></param>
        SPhone.InitiateConference(connID, otherDN);
    },
    CompleteConference: function (connID, conferenceConnID) {
        /// <summary>
        /// 确认会议
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="conferenceConnID"></param>
        SPhone.CompleteConference(connID, conferenceConnID);
    },
    //-----------------------------------------TSVR end-----------------------------------------

    //-----------------------------------------ISVR begin-----------------------------------------
    Connect_ISvr: function () {
        /// <summary>
        /// 连接
        /// </summary>
        SPhone.Connect_ISvr();
    },
    Disconnect_ISvr: function () {
        /// <summary>
        /// 反连接
        /// </summary>
        SPhone.Disconnect_ISvr();
    },
    AgentLogin_ISvr: function () {
        /// <summary>
        /// 登录
        /// </summary>
        SPhone.AgentLogin_ISvr();
    },
    RemoveMedia_ISvr: function (media) {
        /// <summary>
        /// 移除chat
        /// </summary>
        SPhone.RemoveMedia_ISvr(media);
    },
    AgentLogout_ISvr: function () {
        /// <summary>
        /// 注销
        /// </summary>
        SPhone.AgentLogout_ISvr();
    },
    //就绪、未就绪、修改未就绪原因
    CancelNotReadyForMedia_ISvr: function () {
        /// <summary>
        /// 就绪
        /// </summary>
        if (cfg.IServerConfig.AgentStatus != AgentStatus.就绪) {
            SPhone.CancelNotReadyForMedia_ISvr();
        }
        else {
            HandlerIServerEvent('EventAck_Ready', '', 0, '', '');
        }
    },
    NotReadyForMedia_ISvr: function (code, description) {
        /// <summary>
        /// 未就绪
        /// </summary>
        var lenovoReasonCode = "100";
        switch (code) {
            case "-1"://未就绪
                lenovoReasonCode = "100";
                break;
            case "1"://处理电话
                lenovoReasonCode = "100";
                break;
            case "2"://案头工作
                lenovoReasonCode = "100";
                break;
            case "3"://后续跟进
                lenovoReasonCode = "8";
                break;
            case "4"://开会
                lenovoReasonCode = "6";
                break;
            case "5"://培训
                lenovoReasonCode = "7";
                break;
            case "6"://休息
                lenovoReasonCode = "4";
                break;
            case "7"://午餐
                lenovoReasonCode = "5";
                break;
            case "8"://其他
                lenovoReasonCode = "100";
                break;
        }
        if (cfg.IServerConfig.AgentStatus == AgentStatus.就绪) {
            SPhone.NotReadyForMedia_ISvr(code, description, parseInt(lenovoReasonCode));
        }
        else {
            SPhone.ChangeMediaStateReason_ISvr(code, description, parseInt(lenovoReasonCode));
        }
    },
    ChangeMediaStateReason_ISvr: function (code, description, lenovoReasonCode) {
        /// <summary>
        /// 修改未就绪原因
        /// </summary>
        /// <param name="code">参考：AgentStatus</param>
        /// <param name="description">参考：AgentStatus</param>
        SPhone.ChangeMediaStateReason_ISvr(code, description, lenovoReasonCode);
    },
    //接受、拒绝、结束、转接个人、转队列、会议、离开
    Accept_ISvr: function (ticketId, interactionId) {
        /// <summary>
        /// 接受
        /// </summary>
        SPhone.Accept_ISvr(ticketId, interactionId);
    },
    Reject_ISvr: function (ticketId, interactionId) {
        /// <summary>
        /// 拒绝
        /// </summary>
        SPhone.Reject_ISvr(ticketId, interactionId);
    },
    StopProcessing_ISvr: function (interactionId) {
        /// <summary>
        /// 结束
        /// </summary>
        SPhone.StopProcessing_ISvr(interactionId);
    },
    Transfer_ISvr: function (interactionId, agentId, placeId) {
        /// <summary>
        /// 转接个人
        /// </summary>
        /// <param name="interactionId"></param>
        /// <param name="agentId"></param>
        /// <param name="placeId"></param>
        SPhone.Transfer_ISvr(interactionId, agentId, placeId);
    },
    PlaceInQueue_ISvr: function (interactionId, queue) {
        /// <summary>
        /// 转队列
        /// </summary>
        /// <param name="interactionId"></param>
        /// <param name="queue"></param>
        SPhone.PlaceInQueue_ISvr(interactionId, queue);
    },
    Conference_ISvr: function (interactionId, agentId, placeId) {
        /// <summary>
        /// 会议
        /// </summary>
        /// <param name="interactionId"></param>
        /// <param name="agentId"></param>
        /// <param name="placeId"></param>
        SPhone.Conference_ISvr(interactionId, agentId, placeId);
    },
    LeaveInteraction_ISvr: function (interactionId) {
        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="interactionId"></param>
        SPhone.LeaveInteraction_ISvr(interactionId);
    },
    //更新随路数据,获取随路数据
    ChangeProperties_ISvr: function (interactionId, userDataListKeyValueJson) {
        /// <summary>
        /// 更新chat随路数据.因为是新窗口打开，所以改用str传userDataListKeyValueJson
        /// </summary>
        /// <param name="connID"></param>
        /// <param name="userDataListKeyValueJson">[{Key:'',Value:''},{Key:'',Value:''}],可通过utils.keyValueToJson格式化</param>
        SPhone.ChangeProperties_ISvr(interactionId, userDataListKeyValueJson);
    },
    GetUserData_ISvr: function (interactionId) {
        /// <summary>
        /// 获取随路数据，返回json list KeyValue
        /// </summary>
        /// <param name="interactionId"></param>
        /// <returns type=""></returns>
        var jsonstr = SPhone.GetUserData_ISvr(interactionId);
        return JSON.parse(jsonstr);
    }
    //-----------------------------------------ISVR end-----------------------------------------
};

/*cti*/

/* ============================================电话事件============================================ */
function HandlerTServerEvent(name, errorResponse, errorCode, errorMessage) {
    /// <summary>
    /// 处理电话事件
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <param name="errorResponse">返回信息（string）</param>
    /// <param name="errorCode">错误代码 非0则有错误</param>
    /// <param name="errorMessage">错误消息</param>
    if (name != '') {
        if (name != 'EventError') {
            LogResponse('[1]:::::HandlerTServerEvent:::::<br/>' + name);
        }
        else {
            LogException('[1]:::::HandlerTServerEvent:::::<br/>' + name);
        }
    }

    CFG_Merge();
    VF_Merge();

    var client_app_scenario = utils.getUserDataValue(vf.UserData, 'client_app_scenario');

    switch (name) {
        //**************************************Event振铃中**************************************
        case 'EventRinging':
            {
                SoftPhone.SetTalking(true);
                SF_Call0IsEnd = false;
                //判断到底是什么方式传过来的,mute,2步
                if (vf.CallType == CallType.Consult || vf.CallType == CallType.Inbound) {
                    var lenovoConferenceReason = utils.getUserDataValue(vf.UserData, 'LenovoConferenceReason');
                    if (lenovoConferenceReason == '2' || lenovoConferenceReason == '3') { //转个人 会议
                        vf.CallType = CallType.Internal;
                        SoftPhone.VoiceFlowUpdate(vf);//强制
                    }
                    else {//盲转或正常呼入
                        if (vf.TransferConnectionID != null) {
                            vf.ConnectionID = vf.TransferConnectionID;
                            vf.CurrentConnectionID = vf.TransferConnectionID;
                            SoftPhone.VoiceFlowUpdate(vf);
                        }
                    }
                }

                //判断是不是内线
                if (vf.CallType == CallType.Consult || vf.CallType == CallType.Internal || vf.CallType == CallType.Inbound) {
                    setConnection(0, vf.ConnectionID);
                    vf.CallID = utils.getCallID(EmployeeID);
                    if (cfg.TServerConfig.AgentStatus == AgentStatus.就绪) {
                        SoftPhone.AgentNotReadyDefault();
                    }

                    if (vf.CallType == CallType.Consult || vf.CallType == CallType.Inbound) {
                        //显示随路数据
                        var strTargetSkill = utils.getUserDataValue(vf.UserData, 'VirualQueue');//VirualQueue
                        if (strTargetSkill != '') {
                            strTargetSkill = utils.lenovo_trimSkillGroup(strTargetSkill);
                            vf.AgentgroupName = strTargetSkill;//来自队列
                        }
                        var data = '';
                        data += utils.lenovo_foamatSkill(strTargetSkill);
                        data += '/';
                        data += utils.getUserDataValue(vf.UserData, 'ANI');//ANI
                        data += '/' + utils.getUserDataValue(vf.UserData, 'enterIDNum');//enterIDNum
                        $('#rotedata').val(data);

                        //如果有前一个坐席组
                        var strPreAgentgroupName = utils.getUserDataValue(vf.UserData, 'strPreAgentgroupName');//前一个队列
                        if (strPreAgentgroupName != '') {
                            vf.PreAgentgroupName = strPreAgentgroupName;
                        }

                        //如果有前一个坐席
                        var strPreAgentId = utils.getUserDataValue(vf.UserData, 'strPreAgentId');
                        if (strPreAgentId != '') {
                            vf.PreAgentId = strPreAgentId;
                        }
                    }
                    else if (vf.CallType == CallType.Internal) {
                        $('#rotedata').val(vf.OtherDN);
                    }
                    SoftPhone.VoiceFlowUpdate(vf);
                }

                if (utils.getUserDataValue(vf.UserData, 'GSW_PHONE') != '') {
                    SoftPhone.AnswerCall(vf.ConnectionID);
                }
                else {
                    //延迟0.2秒再接起
                    setTimeout(function () { SoftPhone.AnswerCall(vf.ConnectionID); }, 200);
                }
            }
            break;

            //**************************************Event接通成功**************************************
        case 'EventEstablished':
            {
                if (vf.CurrentConnectionID == vf.ConnectionID) {
                    //0:呼入接通,呼出接通
                    var connID = vf.ConnectionID;
                    setConnection(0, connID);//缓存0线ConnectionID
                    if (cfg.TServerConfig.AgentStatus != AgentStatus.处理电话) {
                        SoftPhone.AgentNotReady('1', '处理电话', false);
                    }

                    switchIcon('answer', 'ReleaseCall', '挂断');
                    switchIcon('hold', 'HoldCall', '保持');

                    if (vf.CallType == CallType.Consult || vf.CallType == CallType.Outbound || vf.CallType == CallType.Inbound) {
                        switchIcon('ivr', 'enable', '转满意度');
                        switchIcon('transferSearch', 'enable', '转接');
                        switchIcon('transferTelPay', 'enable', '转电话支付');

                        switchIcon('transfer', 'InitiateTransfer', '转接');
                        switchIcon('conference', 'InitiateConference', '会议');

                        if (vf.CallType == CallType.Outbound) {
                            vf.CallID = utils.getCallID(EmployeeID);

                            //判断是否是自动外拨
                            var GSW_PHONE = utils.getUserDataValue(vf.UserData, 'GSW_PHONE'); //GSW_PHONE
                            var GSW_ActivityID = utils.getUserDataValue(vf.UserData, 'ActivityID'); //GSW_ActivityID
                            var GSW_CustomID = utils.getUserDataValue(vf.UserData, 'CustomerID'); //GSW_CustomID
                            var GSW_SampleID = utils.getUserDataValue(vf.UserData, 'SampleID'); //GSW_SampleID

                            if (GSW_PHONE != '' && utils.getUserDataValue(vf.UserData, 'ANI') == '') {
                                vf.InOut = 3;
                                vf.DNIS = GSW_PHONE.substr(1);
                                vf.ANI = vf.ThisDN;
                            }
                            //end

                            SoftPhone.VoiceFlowUpdate(vf);

                            //自动外拨随路修正
                            if (GSW_PHONE != '' && utils.getUserDataValue(vf.UserData, 'ANI') == '') {
                                SoftPhone.UpdateUserData(connID, [
                                    utils.keyValueToJson('ANI', vf.ThisDN),
                                    utils.keyValueToJson('DNIS', GSW_PHONE.substr(1)),
                                    utils.keyValueToJson('strCustomerID', GSW_CustomID)
                                ]);
                                VF_Merge();
                            }
                            //end

                            //$('#rotedata').val(utils.getUserDataValue(vf.UserData, 'DNIS'));//DNIS

                            $('#rotedata').val(utils.lenovo_getRightDNIS(vf.DNIS));
                        }

                        var outConnID = utils.getUserDataValue(vf.UserData, 'ConnectID');
                        if (outConnID == '') {
                            SoftPhone.UpdateUserData(connID, [utils.keyValueToJson('ConnectID', vf.ConnectionID)]);
                        }

                        if (vf.CallID == null || vf.CallID == '') {
                            vf.CallID = utils.getCallID(EmployeeID);
                            SoftPhone.VoiceFlowUpdate(vf);
                        }

                        //添加随路CallID
                        SoftPhone.UpdateUserData(connID, [
                            utils.keyValueToJson('CallID', vf.CallID),
                            utils.keyValueToJson('strCurCallID', vf.CallID),
                            //添加随路给录用使用
                            utils.keyValueToJson('lenovo_agentId:' + cfg.TServerConfig.DN, EmployeeID),
                            utils.keyValueToJson('lenovo_callId', vf.CallID),
                            utils.keyValueToJson('lenovo_agentIp:' + cfg.TServerConfig.DN, AgentIP)
                        ]);

                        if (vf.InOut == 1) {
                            if (IS_Lenovo_Business) {
                                lenovo_operationDispose(2);
                                if (1 == 2) {
                                    var stateflag = 2;//queue in
                                    //如果有前一个坐席组
                                    var strPreAgentgroupName = utils.getUserDataValue(vf.UserData, 'strPreAgentgroupName');//前一个队列
                                    var strPreAgentId = utils.getUserDataValue(vf.UserData, 'strPreAgentId'); //前一个坐席
                                    if (strPreAgentgroupName != '') {
                                        stateflag = 3;//Trans queue in
                                    }
                                    else if (strPreAgentId != '') {
                                        stateflag = 4;//Trans agent in
                                    }
                                    lenovo_operationDispose(stateflag);
                                }
                            }
                        }
                        else if (vf.InOut == 2) {
                            if (IS_Lenovo_Business) {
                                setTimeout(function () { lenovo_operationDispose(11); }, 100);
                            }
                        }
                        else if (vf.InOut == 3) {
                            if (IS_Lenovo_Business) {
                                setTimeout(function () { lenovo_operationDispose(13); }, 100);
                            }
                        }

                        //合并
                        VF_Merge();

                        LogException('ConnectID(line 845):' + utils.getUserDataValue(vf.UserData, 'ConnectID'));

                        //记录callbegin到数据库
                        var ANI = utils.getUserDataValue(vf.UserData, 'ANI');//主叫
                        var DNIS = utils.getUserDataValue(vf.UserData, 'DNIS');//被叫
                        if (ANI == '') {
                            ANI = vf.ANI;
                        }
                        if (DNIS == '') {
                            DNIS = vf.DNIS;
                        }
                        DNIS = utils.lenovo_getRightDNIS(DNIS);
                        var data = {
                            CallID: vf.CallID,
                            EmployeeID: EmployeeID,
                            ConnectionID: utils.getUserDataValue(vf.UserData, 'ConnectID'),
                            ANI: ANI,
                            DNIS: DNIS,
                            InOut: vf.InOut,
                            CurrentQueueName: vf.AgentgroupName,
                            FromQueueName: vf.PreAgentgroupName
                        };

                        try {
                            lenovoTrace('CallBegin:' + data.CallID + ',' + EmployeeID + ',' + data.ConnectionID);
                        }
                        catch (e) { }

                        $.getJSON(AppServer_DbUri + 'DB/CallCreate?callback=?', data, function (r) {
                            if (r.Code == -1) {
                                LogException(r.Message);
                            }
                        }).error(function () { LogException('DB/CallCreate:失败.CallID=' + vf.CallID) });

                        //记时
                        $('#time1_span').html('通话时间');
                        $('#time1').val('00:00:00');
                        clearInterval(Interval_103);//先清理掉切换状态的记时
                        clearInterval(Interval_102);
                        Interval_102_Reset = -1;
                        var callBeginTime = new Date().getTime();
                        Interval_102 = setInterval(function () {
                            utils.updateTime(callBeginTime, '#time1');
                        }, 1000);
                        Interval_102_Reset = Interval_102;

                        //呼入时报工号
                        var lenovoConferenceReason = utils.getUserDataValue(vf.UserData, 'LenovoConferenceReason');
                        if ((vf.CallType == CallType.Consult || vf.CallType == CallType.Inbound) && vf.InOut == 1 && config_TRANSFERDN.报工号 != '') {
                            if (!(lenovoConferenceReason == '2' || lenovoConferenceReason == '3')) {
                                SoftPhone.UpdateUserData(connID, [
                                                                utils.keyValueToJson('Function', '997'),
                                                                utils.keyValueToJson('Function1', '997'),
                                                                utils.keyValueToJson('IVRAgentID', EmployeeID),
                                                                utils.keyValueToJson('Conference_Type', '1')
                                ]);
                                sayempno.IsSayempno = true;
                                SoftPhone.InitiateConference(connID, config_TRANSFERDN.报工号);
                                SoftPhone.UpdateUserData(connID, [utils.keyValueToJson('client_app_scenario', 'sayempno')]);
                            }
                        }
                    }
                }
                else {
                    //1 ivr接通 转接接通
                    setConnection(1, vf.SecondConnectionID);
                    if ('transfer' == client_app_scenario) {
                        switchIcon('transfer', 'CompleteTransfer', '确认转接');
                        switchIcon('transferCancel', 'ReleaseCall', '取消转接');
                    }
                    else if ('conference' == client_app_scenario) {
                        switchIcon('conference', 'CompleteConference', '确认会议');
                        switchIcon('conferenceCancel', 'ReleaseCall', '取消会议');
                    }
                    else if ('sayempno' == client_app_scenario) {
                        SoftPhone.CompleteConference(getConnection(0), getConnection(1));
                    }
                    else if ('telpay' == client_app_scenario) {
                        SoftPhone.CompleteConference(getConnection(0), getConnection(1));
                    }
                    if ('telpay' != client_app_scenario && client_app_scenario != '') {
                        SoftPhone.UpdateUserData(getConnection(0), [utils.keyValueToJson('client_app_scenario', '')]);
                    }
                }
            }
            break;

            //**************************************Event保持通话**************************************
        case 'EventHeld':
            {
                if ('telpay' == client_app_scenario) {
                    //switchIcon('keypad', 'disabled');
                    switchIcon('hold', 'disabled');
                    switchIcon('transfer', 'disabled');
                    switchIcon('conference', 'disabled');
                    switchIcon('transferSearch', 'disabled');
                    switchIcon('transferTelPay', 'disabled');
                    switchIcon('ivr', 'disabled');
                }
                else {
                    switchIcon('hold', 'RetrieveCall', '取消保持');
                }
            }
            break;

            //**************************************Event取消保持**************************************
        case 'EventRetrieved':
            {
                if ('telpay' == client_app_scenario || 1 == 1) {
                    //switchIcon('keypad', 'enable');
                    //switchIcon('hold', 'enable');
                    switchIcon('transfer', 'InitiateTransfer', '转接');
                    switchIcon('conference', 'InitiateConference', '会议');
                    switchIcon('transferSearch', 'enable', '转接');
                    switchIcon('transferTelPay', 'enable', '电话支付');
                    switchIcon('ivr', 'enable', '转满意度');
                }

                switchIcon('hold', 'HoldCall', '保持通话');

                if (sayempno.IsSayempno) {
                    sayempno.IsRetrieved = true;
                    sayempno.IsSayempno = false;
                }
            }
            break;

            //**************************************Event释放通话**************************************
        case 'EventReleased': {
            LogMessage('释放开始：CurrentConnectionID:' + vf.CurrentConnectionID + '-ConnectionID:' + vf.ConnectionID + '-SecondConnectionID:' + vf.SecondConnectionID);
            LogMessage('Interval_102_Reset:' + Interval_102_Reset + ' - SF_Call0IsEnd:' + SF_Call0IsEnd);

            //记录callend begin 2013.10.31
            try {
                var ConnectID = utils.getUserDataValue(vf.UserData, 'ConnectID');
                lenovoTrace('CallEnd_Assert:{' + vf.CallID + ',' + EmployeeID + ',' + ConnectID + ',Interval_102_Reset:' + Interval_102_Reset + ',SF_Call0IsEnd:' + SF_Call0IsEnd + ',CurrentConnectionID:' + vf.CurrentConnectionID + ',ConnectionID:' + vf.ConnectionID + ',C_C:' + (vf.CurrentConnectionID == vf.ConnectionID && vf.CurrentConnectionID != null) + '}');
            }
            catch (e) { }
            //记录callend end

            if (vf.CurrentConnectionID == null) {
                return;
            }
            if (vf.CurrentConnectionID == vf.ConnectionID) {
                SoftPhone.SetTalking(false);
                SoftPhone.SetDialing(false);
                //挂断0:呼入 呼出
                LogMessage('挂0线begin');
                switchIcon('answer', 'MakeCall', '外拨');
                switchIcon('hold', 'disabled');

                switchIcon('transfer', 'disabled');
                switchIcon('transferCancel', 'disabled');

                switchIcon('conference', 'disabled');
                switchIcon('conferenceCancel', 'disabled');

                switchIcon('ivr', 'disabled');
                switchIcon('transferSearch', 'disabled');
                switchIcon('transferTelPay', 'disabled');

                switchIcon('transferCancel', 'disabled');
                switchIcon('conferenceCancel', 'disabled');

                //呼出时未接通
                if (Interval_102_Reset == -1) {
                    LogMessage('未呼通');
                    setConnection(0, '');
                    switchIcon('agentstatus', 'notready', '未就绪');//需要修改一下，要不然都是禁用的，无法改变图标
                    if (cfg.IServerConfig.AgentStatus == AgentStatus.就绪) {
                        utils.lenovo_beginTimer('文本就绪');
                    }
                    else {
                        utils.lenovo_beginTimer('未就绪');
                    }
                    setAgentStatusChanged();
                }
                else {
                    if (!SF_Call0IsEnd) {
                        SF_Call0IsEnd = true;
                        clearInterval(Interval_102);//清理记时
                        Interval_102_Reset = -1;

                        //更新记录callend到数据库
                        var strCustomerID = utils.getUserDataValue(vf.UserData, 'strCustomerID');

                        var enddata = {
                            CallID: vf.CallID,
                            EmployeeID: EmployeeID,
                            CustomerID: strCustomerID,
                            NextQueueName: vf.NextAgentgroupName,
                            IsConference: vf.IsConference,
                            IsTransfer: vf.IsTransfer,
                            IsTransferEPOS: vf.IsTransferEPOS
                        };

                        try {
                            var ConnectID = utils.getUserDataValue(vf.UserData, 'ConnectID');
                            lenovoTrace('CallEnd:' + vf.CallID + ',' + EmployeeID + ',' + ConnectID);
                        }
                        catch (e) { }

                        $.getJSON(AppServer_DbUri + 'Db/CallEnd?callback=?', enddata, function (r) { })
                            .error(function () { LogException('DB/CallEnd:发生错误.callid=' + enddata.CallID); });
                        SF_WaitingDeskTime_CallID = vf.CallID;

                        SoftPhone.AgentNotReady('2', '案头工作', true);

                        if (IS_Lenovo_Business) {
                            lenovo_operationDispose(7);
                        }

                        //$('#rotedata').val('');
                    }
                }
                setConnection(0, '');
                SPhone.VoiceFlowReset();
            }
            else if (vf.CurrentConnectionID == vf.SecondConnectionID || vf.CurrentConnectionID == getConnection(1)) {
                //挂断1线
                SoftPhone.SetDialing(false);
                LogMessage('挂1线begin');
                switchIcon('transfer', 'InitiateTransfer', '转接');
                switchIcon('transferCancel', 'disabled');

                switchIcon('conference', 'InitiateConference', '会议');
                switchIcon('conferenceCancel', 'disabled');

                if ('telpay' != client_app_scenario && client_app_scenario != '') {
                    SoftPhone.UpdateUserData(vf.ConnectionID, [utils.keyValueToJson('client_app_scenario', '')]);//取消场景
                }

                if (getConnection(0) == undefined) {
                    switchIcon('transfer', 'disabled');
                    switchIcon('conference', 'disabled');
                }

                setConnection(1, '');

                if (sayempno.IsSayempno) {
                    setTimeout(function () {
                        if (sayempno.IsSayempno && !sayempno.IsRetrieved) {
                            sayempno.IsRetrieved = true;
                            sayempno.IsSayempno = false;
                            var connID = getConnection(0);
                            if (connID) {
                                SoftPhone.RetrieveCall(connID);
                            }
                        }
                    }, 100);
                }
            }
        }
            break;

            //**************************************Event挂机**************************************
        case 'EventOnHook': {
            HandlerTServerEvent('EventReleased', errorResponse, errorCode, errorMessage);
        }
            break;

            //**************************************Event振铃后未接通，用户挂断**************************************
        case 'EventAbandoned': {
            HandlerTServerEvent('EventReleased', errorResponse, errorCode, errorMessage);
        }
            break;

            //**************************************Event就绪成功**************************************
        case 'EventAgentReady':
            {
                if (AgentStatusClick_Voice != '') {
                    AgentStatusClick_Voice = '';
                    setMenuSelected(0);
                }
                var d = utils.getAgentStatus(cfg.TServerConfig.AgentStatus);
                if (d.Value == 0) {
                    if (IS_Lenovo_Business) {
                        lenovo_operationDispose(8);
                    }
                }
                if (SF_WaitingDeskTime_CallID != '' && SF_WaitingDeskTime_CallID != null) {
                    var data = {
                        CallID: SF_WaitingDeskTime_CallID,
                        EmployeeID: EmployeeID
                    }
                    $.getJSON(AppServer_DbUri + 'Db/CallSetDeskTime?callback=?', data, function (r) { });
                    if (IS_Lenovo_Business) {
                        //lenovo_operationDispose(7);
                    }
                    SF_Call0IsEnd = false;
                    SF_WaitingDeskTime_CallID = '';
                }

                switchIcon('agentstatus', 'voiceready', '语音就绪');
                if (IsVoiceOnOffClick) {
                    IsVoiceOnOffClick = false;
                    topJ('#v2_lenovo_voice_onoff').attr('checked', true);
                }
                if (cfg.IServerConfig.AgentStatus == AgentStatus.就绪) {
                    utils.lenovo_beginTimer('就绪');
                }
                else {
                    utils.lenovo_beginTimer('语音就绪');
                }
                setAgentStatusChanged();
            }
            break;

            //**************************************Event未就绪成功**************************************
        case 'EventAgentNotReady':
            {
                var d = utils.getAgentStatus(cfg.TServerConfig.AgentStatus);

                LogMessage('EventAgentNotReady(line:1003) begin:' + d.Value);

                if (d.Value == AgentStatus.其他) {
                    AgentStatusClick_Voice = '其他';
                }

                //$('#voice_current_status').html(d.Key);
                if (d.Value == AgentStatus.案头工作) {
                    switchIcon('agentstatus', 'anmian', '案头工作');
                    utils.lenovo_beginTimer('案面时间');
                }
                else {
                    if (getConnection(0) || d.Value == AgentStatus.处理电话) {
                        $('#agentstatus span:first').attr('class', 'disabled');
                        topJ('#v2_menu').hide();
                    }
                    else {
                        switchIcon('agentstatus', 'notready', d.Key);
                        setAgentStatusChanged();
                        if (IsVoiceOnOffClick) {
                            IsVoiceOnOffClick = false;
                            topJ('#v2_lenovo_voice_onoff').attr('checked', false);

                            if (utils.lenovo_isOn('#v2_lenovo_chat_onoff')) {
                                if (cfg.IServerConfig.AgentStatus == AgentStatus.就绪) {
                                    utils.lenovo_beginTimer('文本就绪');
                                }
                                else {
                                    var d = utils.getAgentStatus(cfg.IServerConfig.AgentStatus);
                                    utils.lenovo_beginTimer(d.Key);
                                    if (d.Value == -1) {
                                        switchIcon('agentstatus', 'notready', d.Key);
                                    }
                                    else {
                                        switchIcon('agentstatus', 'notready_' + d.Value, d.Key);
                                    }
                                }
                            }
                            else {
                                utils.lenovo_beginTimer('未就绪');
                            }
                        }
                        if (AgentStatusClick_Voice != '') {
                            utils.lenovo_beginTimer(AgentStatusClick_Voice);
                            switchIcon('agentstatus', 'notready_' + d.Value, AgentStatusClick_Voice);
                            setMenuSelected(d.Value);
                            AgentStatusClick_Voice = '';
                        }
                    }
                }
                if (IS_Lenovo_Business) {
                    lenovo_operationDispose(9);
                }
                if (d.Value != 2 && d.Value != 1 && SF_WaitingDeskTime_CallID != '' && SF_WaitingDeskTime_CallID != null) {
                    var data = {
                        CallID: SF_WaitingDeskTime_CallID,
                        EmployeeID: EmployeeID
                    }
                    $.getJSON(AppServer_DbUri + 'Db/CallSetDeskTime?callback=?', data, function (r) { });
                    if (IS_Lenovo_Business) {
                        //lenovo_operationDispose(7);
                    }
                    SF_Call0IsEnd = false;
                    SF_WaitingDeskTime_CallID = '';
                }
            }
            break;

            //**************************************Event拨打中**************************************
        case 'EventDialing':
            {
                if (vf.CallType == CallType.Consult) {
                    setConnection(1, vf.SecondConnectionID);
                }
                else {
                    setConnection(0, vf.ConnectionID);
                }

                //callback: 拨打成功 2015.01.06
                if (utils.getUserDataValue(vf.UserData, 'strReserveCallBackID') != '') {
                    reserveCallBack.callBackID = '';
                    reserveCallBack.isInCurrent = true;
                    setTimeout(function () { lenovo_operationDispose(14); }, 100);
                }

                if (vf.CurrentConnectionID == vf.ConnectionID) {
                    SoftPhone.AgentNotReady('-1', '未就绪', false);
                }
                else {
                    if ('transfer' == client_app_scenario) {
                        switchIcon('transfer', 'disabled');
                        switchIcon('transferCancel', 'ReleaseCall', '取消转接');

                        switchIcon('conference', 'disabled');
                    }
                    else if ('conference' == client_app_scenario) {
                        switchIcon('conference', 'disabled');
                        switchIcon('conferenceCancel', 'ReleaseCall', '取消会议');

                        switchIcon('transfer', 'disabled');
                    }
                    else if ('telpay' == client_app_scenario) {
                        LogMessage('telpay拨打中:' + getConnection(1));
                    }
                }
            }
            break;

            //**************************************1.Event连接成功**************************************
        case 'EventLinkConnected':
            {
                if (cfg.TServerConfig.IsLinkConnected && !cfg.TServerConfig.IsRegistered && !cfg.TServerConfig.Exit && !VOICE_Invalid) {
                    setTimeout(function () { SoftPhone.RegisterAddress(); }, 200);
                    Interval_100 = setTimeout(function () {
                        HandlerTServerEvent('EventLinkConnected', '检测是否注册成功', 0, '');
                    }, 3000);
                }
                else {
                    clearTimeout(Interval_100);
                    Interval_100 = -1;
                }
            }
            break;

            //**************************************2.Event注册成功**************************************
        case 'EventRegistered': {
            if (cfg.TServerConfig.IsLinkConnected && !cfg.TServerConfig.IsAgentLogin && !cfg.TServerConfig.Exit && !VOICE_Invalid) {
                setTimeout(function () { SoftPhone.AgentLogin(); }, 200);
                Interval_101 = setTimeout(function () {
                    HandlerTServerEvent('EventRegistered', '检测是否登录成功', 0, '');
                }, 5000);
            }
            else {
                clearTimeout(Interval_101);
                Interval_101 = -1;
            }
        }
            break;

            //**************************************3.Event登录成功**************************************
        case 'EventAgentLogin': {
            topJ('#v2_lenovo_voice_onoff').removeAttr('disabled');
            switchIcon('keypad', 'enable', '拨号盘');
            switchIcon('answer', 'MakeCall', '外拨');
            switchIcon('agentstatus', 'enable', '更改坐席状态');
            if (ToolBarSet == ToolBarUseType.ESD) {
                SoftPhone.AgentNotReadyDefault();
            }
            else {
                if (IsCCNowTest) {
                    SoftPhone.AgentNotReadyDefault();
                }
                else {
                    SoftPhone.AgentReady();
                }
            }
            $('#softphoneblockui').unblock();
            VOICE_Invalid = false;
            ReLoginError = false;
            utils.lenovo_beginTimer('就绪');
        }
            break;

            //**************************************4.Event注销成功**************************************
        case 'EventAgentLogout':
            {
                //非正常退出,如果是正常退出不会收到这个事件;如果是620错误，继续登录。
                if (cfg.TServerConfig.IsAgentLogin) {
                    if (ReLoginError && !VOICE_Invalid) {
                        setTimeout(function () {
                            SoftPhone.AgentLogin();
                        }, 500);
                        return;
                    }
                    topJ('#v2_lenovo_voice_onoff').attr('disabled', true);
                    AgentLogout_AbNormal = true;
                    VOICE_Invalid = true;

                    SoftPhone.UnregisterAddress();

                    $.growlUI('SoftPhone Notification', '话机已退出,请重新登录');

                    setAgentStatusChanged();
                    //switchIcon('agentstatus', 'disabled');

                    //-----------------
                    switchIcon('answer', 'disabled');
                    switchIcon('keypad', 'disabled');
                    switchIcon('hold', 'disabled');
                    //-----------------
                    switchIcon('transfer', 'disabled');
                    switchIcon('transferCancel', 'disabled');

                    switchIcon('conference', 'disabled');
                    switchIcon('conferenceCancel', 'disabled');
                    //-----------------
                    switchIcon('ivr', 'disabled');
                    switchIcon('transferSearch', 'disabled');
                    switchIcon('transferTelPay', 'disabled');
                    //-----------------
                    switchIcon('transferCancel', 'disabled');
                    switchIcon('conferenceCancel', 'disabled');

                    var data = {
                        personDBID: PERSON_DBID
                    };
                    $.getJSON(AppSoftPhoneServiceUri + 'PhoneStat/AgentLogout_Voice?callback=?', data, function (r) { });
                }
            }
            break;

            //**************************************Event转接给我的**************************************
        case 'EventPartyChanged': {
            setConnection(0, vf.ConnectionID);
            var connID = getConnection(0);

            var lenovoConferenceReason = utils.getUserDataValue(vf.UserData, 'LenovoConferenceReason');
            var isConsult = false;
            if ((vf.CallType == CallType.Inbound || vf.CallType == CallType.Outbound) && (lenovoConferenceReason == '2' || lenovoConferenceReason == '3')) {
                isConsult = true;

                //显示随路数据
                if (vf.CallType == CallType.Inbound) {
                    var strTargetSkill = utils.getUserDataValue(vf.UserData, 'VirualQueue');//VirualQueue
                    if (strTargetSkill != '') {
                        strTargetSkill = utils.lenovo_trimSkillGroup(strTargetSkill);
                        vf.AgentgroupName = strTargetSkill;//来自队列
                    }
                    var data = '';
                    data += utils.lenovo_foamatSkill(strTargetSkill);
                    data += '/';
                    data += utils.getUserDataValue(vf.UserData, 'ANI');//ANI
                    data += '/' + utils.getUserDataValue(vf.UserData, 'enterIDNum');//enterIDNum
                    $('#rotedata').val(data);
                }
                else if (vf.CallType == CallType.Outbound) {
                    var data = '';
                    data += utils.getUserDataValue(vf.UserData, 'DNIS');//DNIS
                    $('#rotedata').val(data);
                }

                vf.CallType = CallType.Consult;
                SoftPhone.VoiceFlowUpdate(vf);//强制

                //如果有前一个坐席组
                var strPreAgentgroupName = utils.getUserDataValue(vf.UserData, 'strPreAgentgroupName');//前一个队列
                if (strPreAgentgroupName != '') {
                    vf.PreAgentgroupName = strPreAgentgroupName;
                }

                //如果有前一个坐席
                var strPreAgentId = utils.getUserDataValue(vf.UserData, 'strPreAgentId');
                if (strPreAgentId != '') {
                    vf.PreAgentId = strPreAgentId;
                }
                SoftPhone.VoiceFlowUpdate(vf);
            }
            if (isConsult) {
                HandlerTServerEvent('EventEstablished', errorResponse, errorCode, errorMessage);
            }
        }
            break;

            //**************************************Event多方通话 有人加入**************************************
        case 'EventPartyAdded': {
            //如果是电话支付，把自己hold住，同时只能挂机
            LogMessage('如果是电话支付，把自己hold住，同时只能挂机.client_app_scenario:' + client_app_scenario + ' connID:' + getConnection(0));
            if ('telpay' == client_app_scenario) {
                var connID = getConnection(0);
                if (connID) {
                    SoftPhone.HoldCall(connID);
                }
            }
        }
            break;

            //**************************************Event多方通话 有人退出**************************************
        case 'EventPartyDeleted': {
            //如果是电话支付，取回通话，恢复其他.清空电话支付场景
            if ('telpay' == client_app_scenario) {

                var connID = getConnection(0);
                SoftPhone.UpdateUserData(connID, [utils.keyValueToJson('client_app_scenario', '')]);

                var thirdPartyDN = vf.ThirdPartyDN;
                if (thirdPartyDN.length == 7 && '763' == thirdPartyDN.substring(0, 3)) {
                    if (connID) {
                        SoftPhone.RetrieveCall(connID);
                    }
                }
                else {
                    $.growlUI('SoftPhone Notification', '客户挂断');
                    if (connID) {
                        SoftPhone.RetrieveCall(connID);
                        SoftPhone.ReleaseCall(connID);
                    }
                }
            }
        }
            break;

            //**************************************Event调用API时出错**************************************
        case 'EventError': {
            //613 - 说明注销不了:不再发起登录
            //615 - 电话还在通话中，请先挂断
            //619 - Administration has removed the domain.话机离线
            //621 - 别人已经登录了，不用再登录
            //629 - 无法将外拨的电话转给外部电话
            //56 - Client application is requesting a function and specifying an invalid connectionID.
            //58 - 不是真实的分机不在服务内
            LogException('softphone_api.js 1031:[' + errorCode + ':' + errorMessage + ':' + errorResponse + ']');
            if (errorResponse.indexOf("Genesyslab.Platform.Commons.Protocols.ProtocolException: Connection is not opened") != -1) {
                LogMessage('正在尝试：重新连接');
                setTimeout(function () { SoftPhone.Connect(); }, 2000);
            }

            if (errorCode == 620 || errorMessage == 'Sign-in number is already active at another console') {
                if (!VOICE_Invalid && !AgentLogout_AbNormal) {
                    LogMessage('错误:620 正在尝试：重新登录');
                    LogMessage('VOICE_Invalid:' + VOICE_Invalid);
                    ReLoginError = true;
                    if (ToolBarSet != ToolBarUseType.ESD) {
                        setTimeout(function () { SoftPhone.AgentLogout(); }, 1000);//登录不上，重新注销再登录
                    }
                    else {
                        VOICE_Invalid = true;
                        clearTimeout(Interval_101);
                        Interval_101 = -1;
                        SoftPhone.UnregisterAddress();
                    }
                }
            }
            else if (errorCode == 621) {
                VOICE_Invalid = true;
                $('#v2_lenovo_voice_onoff').attr('disabled', true);
                SoftPhone.UnregisterAddress();
                if (cfg.IServerConfig.EnableChat && cfg.IServerConfig.IsLinkConnected) {
                    SoftPhone.Disconnect_ISvr();
                }
                $.growlUI('SoftPhone Notification', '坐席已经登录');
                $('#softphoneblockui').unblock();

                switchIcon('agentstatus', 'disabled');
                //-----------------
                switchIcon('answer', 'disabled');
                switchIcon('keypad', 'disabled');
                switchIcon('hold', 'disabled');
                //-----------------
                switchIcon('transfer', 'disabled');
                switchIcon('transferCancel', 'disabled');

                switchIcon('conference', 'disabled');
                switchIcon('conferenceCancel', 'disabled');
                //-----------------
                switchIcon('ivr', 'disabled');
                switchIcon('transferSearch', 'disabled');
                switchIcon('transferTelPay', 'disabled');
                //-----------------
                switchIcon('transferCancel', 'disabled');
                switchIcon('conferenceCancel', 'disabled');

                //LogMessage('更改坐席状态图标');
                //setAgentStatusChanged();
            }
            else if (errorCode == 613 || errorCode == 619 || errorCode == 58) {
                LogMessage('错误:' + errorCode);
                VOICE_Invalid = true;
                topJ('#v2_lenovo_voice_onoff').attr('disabled', true);

                SoftPhone.UnregisterAddress();

                $.growlUI('SoftPhone Notification', '话机异常:' + errorCode);

                //switchIcon('agentstatus', 'disabled');
                //-----------------
                switchIcon('answer', 'disabled');
                switchIcon('keypad', 'disabled');
                switchIcon('hold', 'disabled');
                //-----------------
                switchIcon('transfer', 'disabled');
                switchIcon('transferCancel', 'disabled');

                switchIcon('conference', 'disabled');
                switchIcon('conferenceCancel', 'disabled');
                //-----------------
                switchIcon('ivr', 'disabled');
                switchIcon('transferSearch', 'disabled');
                switchIcon('transferTelPay', 'disabled');
                //-----------------
                switchIcon('transferCancel', 'disabled');
                switchIcon('conferenceCancel', 'disabled');

                LogMessage('更改坐席状态图标');
                setAgentStatusChanged();

                if (!cfg.IServerConfig.EnableChat || Chat_Invalid) {
                    switchIcon('agentstatus', 'disabled');
                    $('#softphoneblockui').unblock();
                }
            }
            else if (errorCode == 615) {

            }
            else if (errorCode == 41) {
                if (!AgentLogout_AbNormal) {
                    LogMessage('错误:41 正在尝试：重新注册');
                    setTimeout(function () { SoftPhone.RegisterAddress(); }, 2000);
                }
            }
            else if (errorCode == 185) {
                if (!AgentLogout_AbNormal) {
                    LogMessage('错误185 正在尝试：重新登录');
                    setTimeout(function () { SoftPhone.AgentLogin(); }, 2000);
                }
            }
            else if (errorCode == 629) {
                $.growlUI('SoftPhone Notification', '无法操作');
            }
            else if (errorMessage == 'Invalid Called Dn') {
                $.growlUI('SoftPhone Notification', 'Invalid Called Dn');
            }
            else if (errorCode == 56 || errorCode == 470)//Invalid connection id
            {
                if (getConnection(0) == undefined) {
                    $.growlUI('SoftPhone Notification', '用户已挂断');
                    switchIcon('agentstatus', 'anmian', '案头工作');
                    utils.lenovo_beginTimer('案面时间');

                    switchIcon('answer', 'MakeCall', '外拨');
                    switchIcon('hold', 'disabled');

                    switchIcon('transfer', 'disabled');
                    switchIcon('transferCancel', 'disabled');

                    switchIcon('conference', 'disabled');
                    switchIcon('conferenceCancel', 'disabled');


                    switchIcon('ivr', 'disabled');

                    switchIcon('transferSearch', 'disabled');
                    switchIcon('transferTelPay', 'disabled');
                }
            }
            else if (errorCode == 605) {
                $.growlUI('SoftPhone Notification', '无效的电话号码');
                switchIcon('answer', 'MakeCall', '拨打');
            }
            else {
                if (errorMessage) {
                    LogException('softphone_api.js line 1408:' + errorMessage);
                }
            }
        }
            break;
    }
}

/* ============================================Chat事件============================================ */
function HandlerIServerEvent(name, errorResponse, errorCode, errorMessage, interactionId) {
    if (name != '') {
        if (name != 'EventError') {
            LogResponse('[2]:::::HandlerIServerEvent:::::<br/>' + name);
        }
        else {
            LogMessage('[2]:::::HandlerIServerEvent:::::<br/>' + name);
        }
    }
    CFG_Merge();
    switch (name) {
        //**************************************有响应**************************************
        case 'EventAck': {
        }
            break;

            //**************************************有新的交互**************************************
        case 'EventInvite': {
            if (cfg.IServerConfig.IsLinkConnected == false) {
                return;
            }
            var cf = JSON.parse(SPhone.GetChatFlow(interactionId));

            SoftPhone.Accept_ISvr(cf.TicketId, cf.InteractionId);

            LogMessage('新的交互信息：' + cf);

            //闪动3秒，然后打开窗口
            switchIcon('chat', 'flash', 'chat');

            openChatWin(cf.TicketId, cf.InteractionId, cf.UserData);//cti.js

            setTimeout(function () {
                switchIcon('chat', 'enable', 'chat');
            }, 3000);
        }
            break;

            //**************************************就绪**************************************
        case 'EventAck_Ready': {
            Chat_LastAgentState.code = '';
            Chat_LastAgentState.description = '';
            if (AgentStatusClick_Chat != '') {
                AgentStatusClick_Chat = '';
                setMenuSelected(0);
            }

            if (IS_Lenovo_Business) {
                lenovo_operationDispose(8);
            }

            switchIcon('agentstatus', 'chatready', '文本就绪');
            if (IsChatOnOffClick) {
                IsChatOnOffClick = false;
                topJ('#v2_lenovo_chat_onoff').attr('checked', true);
            }
            if (cfg.TServerConfig.AgentStatus == AgentStatus.就绪 && !VOICE_Invalid) {
                utils.lenovo_beginTimer('就绪');
            }
            else {
                utils.lenovo_beginTimer('文本就绪');
            }
            setAgentStatusChanged();
        }
            break;

            //**************************************未就绪**************************************
        case 'EventAck_NotReady': {
            switchIcon('agentstatus', 'notready', '未就绪');

            var d = utils.getAgentStatus(cfg.IServerConfig.AgentStatus);
            Chat_LastAgentState.code = d.Value;
            Chat_LastAgentState.description = d.Key;

            if (IsChatOnOffClick) {
                IsChatOnOffClick = false;
                topJ('#v2_lenovo_chat_onoff').attr('checked', false);

                setAgentStatusChanged();
                if (utils.lenovo_isOn('#v2_lenovo_voice_onoff')) {
                    if (cfg.TServerConfig.AgentStatus == AgentStatus.就绪) {
                        utils.lenovo_beginTimer('语音就绪');
                    }
                    else {
                        LogMessage('api(1452)');
                        var d2 = utils.getAgentStatus(cfg.TServerConfig.AgentStatus);
                        utils.lenovo_beginTimer(d2.Key);
                        if (d2.Value == -1) {
                            switchIcon('agentstatus', 'notready', d2.Key);
                        }
                        else {
                            switchIcon('agentstatus', 'notready_' + d2.Value, d2.Key);
                        }
                    }
                }
                else {
                    utils.lenovo_beginTimer('未就绪');
                }
            }
            else {
                setAgentStatusChanged();
                if (AgentStatusClick_Chat != '') {
                    utils.lenovo_beginTimer(AgentStatusClick_Chat);
                    setMenuSelected(d.Value);
                    if (d.Value == -1) {
                        switchIcon('agentstatus', 'notready', d.Key);
                    }
                    else {
                        switchIcon('agentstatus', 'notready_' + d.Value, d.Key);
                    }
                    AgentStatusClick_Chat = '';
                }
            }

            //$('#chat_current_status').html(d.Key);
            if (!cfg.TServerConfig.EnableVoice) {
                //switchIcon('agentstatus', 'notready', '未就绪');
            }
            if (IS_Lenovo_Business) {
                lenovo_operationDispose(9);
            }
        }
            break;
            //    //**************************************已经交互**************************************
            //case 'EventAgentInvited': {
            //}
            //    break;

            //    //**************************************已接受**************************************
            //case 'EventAccepted': {
            //}
            //    break;

            //**************************************结束**************************************
        case 'EventRevoked': {
            try {
                chatWin.eventRevoked(interactionId);// chat/chat_api.js
            }
            catch (ex) { }
        }
            break;

            //**************************************打开连接成功**************************************
        case 'MediaCall_Opened': {

            if (Chat_Ws_ReLogin) {
                Chat_Ws_ReLogin = false;
                LogMessage('主备:打开后继续重连');
                SoftPhone.AgentLogin_ISvr();

                setTimeout(function () {
                    //修改状态
                    if (Chat_LastAgentState.code != '') {
                        AgentStatusClick_Chat = Chat_LastAgentState.description;
                        SoftPhone.NotReadyForMedia_ISvr(Chat_LastAgentState.code, Chat_LastAgentState.description);
                        //HandlerIServerEvent('EventAck_NotReady', '更改状态条');
                    }
                }, 500);
            }

            if (!cfg.IServerConfig.IsAgentLogin && !cfg.IServerConfig.Exit && !Chat_Invalid) {
                setTimeout(function () {
                    LogMessage('1.尝试登录');
                    SoftPhone.AgentLogin_ISvr();
                }, 100);
                Interval_200 = setTimeout(function () {
                    LogMessage('2.检测是否登录');
                    HandlerIServerEvent('MediaCall_Opened', '检测是否登录成功');
                }, 3000);
            }
            else {
                clearTimeout(Interval_200);
            }
        }
            break;

            //**************************************登录成功后**************************************
        case 'EventAck_AgentLogin': {
            topJ('#v2_lenovo_chat_onoff').removeAttr('disabled');
            switchIcon('agentstatus', 'ready', '就绪');
            setAgentStatusChanged();
            switchIcon('chat', 'enable', 'chat');
            $('#softphoneblockui').unblock();
            if (IsCCNowTest) {
                SoftPhone.NotReadyForMedia_ISvr('-1', '未就绪');
            }

            utils.lenovo_beginTimer('就绪');
        }
            break;

            //**************************************连接丢失**************************************
        case 'EventAgentConnectionClosed': {
            switchIcon('chat', 'disabled');
        }
            break;

        case 'EventError': {
            //12.被占用
            //14.Agent already logged in 无法登录
            //34.Client is not logged in 无法注销

            //登录不上，重新注销再登录
            if (errorCode == 12) {

            }
            if (errorCode == 14) {
                if (!Chat_Invalid) {
                    $.growlUI('SoftPhone Notification', '坐席文本已登录,尝试重新登录');
                    setTimeout(function () {
                        SoftPhone.AgentLogout_ISvr();
                        setTimeout(function () { SoftPhone.AgentLogin_ISvr(); }, 200);
                    }, 2000);
                }
            }
            else if (errorCode == 34) {
                Chat_Invalid = true;
            }
        }
            break;
    }

}

function TServerReTryConnect() {
    /// <summary>
    /// ocx:TSERVER_Closed时自动重新连接
    /// </summary>
    if (!VOICE_Invalid) {
        setTimeout(function () {
            SoftPhone.Connect();
        }, 2000);
    }
}

function IServerReTryConnect() {
    /// <summary>
    /// ocx:ISERVER_Closed时自动重新连接
    /// </summary>
    if (!Chat_Invalid) {
        setTimeout(function () {
            LogMessage('IServerReTryConnect');
            SoftPhone.Connect_ISvr();
        }, 2000);
    }
}

function MediaCall_Closed() {
    Chat_Ws_ReLogin = true;
}

function GetCurrentTime() {
    /// <summary>
    /// yyyyMMddHHmmss
    /// </summary>
    /// <returns type="string"></returns>
    var d = new Date();
    var year = d.getFullYear().toString();
    var month = "0" + (d.getMonth() + 1);
    var day = '0' + d.getDate();
    var hour = '0' + d.getHours();
    var minute = '0' + d.getMinutes();
    var second = '0' + d.getSeconds();
    var result = year +
        month.substr(month.length - 2) +
        day.substr(day.length - 2) +
        hour.substr(hour.length - 2) +
        minute.substr(minute.length - 2) +
        second.substr(second.length - 2);
    return result;
}

//工具类
var utils = {
    secondFormat: function (seconds) {
        /// <summary>
        /// 秒格式化:01:01:01
        /// </summary>
        /// <param name="seconds">2000</param>
        /// <returns type="string">01:01:01</returns>
        var d = new Date(seconds * 1000);
        var hour = '0' + d.getUTCHours();
        var minute = '0' + d.getMinutes();
        var second = '0' + d.getSeconds();
        var result =
        hour.substr(hour.length - 2) + ':' +
        minute.substr(minute.length - 2) + ':' +
        second.substr(second.length - 2);
        return result;
    },
    getUserDataValue: function (vfUserData, key) {
        /// <summary>
        /// 获取随路数据,如果没有找到返回 ''
        /// </summary>
        /// <param name="vfUserData"></param>
        /// <param name="key"></param>
        /// <returns type="String"></returns>
        var val = '';
        $(vfUserData).each(function (index, e) {
            if (this.Key.toLowerCase() == key.toLowerCase()) {
                val = this.Value;
                return false;
            }
        });
        if (val == null) {
            val = '';
        }
        return val;
    },
    keyValueToJson: function (key, value) {
        /// <summary>
        /// 将key value转换为json
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns type=""></returns>
        return { Key: key, Value: value };
    },
    updateTime: function (t, selector) {
        /// <summary>
        /// 计时开始
        /// </summary>
        /// <param name="t">开始时间戳</param>
        /// <param name="selector">需要更新的html</param>
        var d = (new Date().getTime() - t) / 1000;
        $(selector).val(utils.secondFormat(d));
    },
    getAgentStatus: function (val) {
        /// <summary>
        /// 获取坐席状态
        /// </summary>
        /// <param name="val">数值</param>
        /// <returns type="json"></returns>
        for (var item in AgentStatus) {
            if (AgentStatus[item] == val) {
                return { Key: item, Value: val };
            }
        }
        return { Key: '未知', Value: -100 }
    },
    getCallID: function (agentID) {
        /// <summary>
        /// 获取CallID
        /// </summary>
        /// <param name="agentID">员工编号</param>
        /// <returns type="String">string</returns>
        return agentID + GetCurrentTime();
    },
    lenovo_trimSkillGroup: function (s) {
        /// <summary>
        /// 去掉V
        /// </summary>
        /// <param name="s"></param>
        /// <returns type=""></returns>
        if (s && s.substring(0, 1).toUpperCase() == "V")
            s = s.substring(1);
        else
            s = s;
        return s;
    },
    lenovo_getRightPhoneNumber: function (phoneNumber) {
        /// <summary>
        /// 返回加了授权码的可以拨出的号码
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns type=""></returns>
        if (phoneNumber && phoneNumber.length > 0) {
            phoneNumber = $.trim(phoneNumber);
        }
        if (phoneNumber && phoneNumber.length > 0) {
            if ("#" == phoneNumber.substring(0, 1)) {
                return phoneNumber;
            }
            else if (phoneNumber.indexOf(",") > 0) {
                return phoneNumber;
            }
            else {
                return '9' + phoneNumber + "#,,," + lenovo_authorizedNumber;//96050904#
            }
        }
        else {
            return phoneNumber;
        }
    },
    lenovo_getRightDNIS: function (DNIS) {
        /// <summary>
        /// 获取正确的被叫号码
        /// </summary>
        /// <param name="DNIS">被叫号码</param>
        /// <returns type=""></returns>
        if (DNIS && DNIS.length > 0) {
            if ("#" == DNIS.substring(0, 1)) {
                return DNIS.substring(1);
            }
            else if ('9' == DNIS.substring(0, 1) && DNIS.indexOf("#,,,") > 0) {
                return DNIS.substring(1).split('#,,,')[0];
            }
            else {
                return DNIS;
            }
        }
        else {
            return DNIS;
        }
    },
    lenovo_isOn: function (selector) {
        return top.$(selector).prop('disabled') == false && top.$(selector).prop('checked');
    },
    lenovo_beginTimer: function (status) {
        /// <summary>
        /// 开始记时
        /// </summary>
        /// <param name="status"></param>
        var beginTime = new Date().getTime();
        clearInterval(Interval_103);
        $('#time1').val('00:00:00');
        Interval_103 = setInterval(function () { utils.updateTime(beginTime, '#time1'); }, 1000);
        $('#time1_span').html(status);
    },
    lenovo_foamatSkill: function (skill) {
        if (skill == null || skill == '') { return skill; }
        var arr = skill.split('_');
        var isnj = false;
        if (arr[arr.length - 1].toLowerCase() == 'NJ'.toLowerCase()) {
            isnj = true;
        }
        return (!isnj) ? arr[0] : arr[0] + '_NJ';
    }
};