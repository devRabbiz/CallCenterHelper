/// <reference path="jquery-1.9.1.js" />
/// <reference path="softphone_api.js" />
/// <reference path="softphone_interface.js" />
/// <reference path="softphone_config.js" />
/// <reference path="jquery-ui-1.10.1.js" />
/// <reference path="common.js" />
/// <reference path="jquery.blockUI.js" />

//禁止右键,禁止选择
$(function () {
    if (!LENOVODEBUG) {
        $(document).bind('contextmenu', function () { return false; });
        $(document).keydown(function () {
            if (event.keyCode == 116) {
                event.keyCode = 0;
                event.returnValue = false;
            }
        });
    }
});

//初始化菜单
$(function () {
    //$('#menu').width(150);
    //$('#menu').menu({
    //    role: 'menuitem',
    //    select: function (event, ui) {
    //        if ($('a:first', ui.item).hasClass('click')) {
    //            var cmd = $('a:first', ui.item).attr('jtag');
    //            switch (cmd) {
    //                case 'all:0': {
    //                    var d = utils.getAgentStatus(cmd.split(':')[1]);
    //                    if (cfg.TServerConfig.EnableVoice && !VOICE_Invalid) {
    //                        SoftPhone.AgentReady();
    //                    }
    //                    if (cfg.IServerConfig.EnableChat) {
    //                        SoftPhone.CancelNotReadyForMedia_ISvr();
    //                    }
    //                }
    //                    break;
    //                case 'all:-1': {
    //                    var d = utils.getAgentStatus(cmd.split(':')[1]);
    //                    if (cfg.TServerConfig.EnableVoice && !VOICE_Invalid) {
    //                        SoftPhone.AgentNotReady(d.Value, d.Key, false);
    //                    }
    //                    if (cfg.IServerConfig.EnableChat) {
    //                        SoftPhone.NotReadyForMedia_ISvr(d.Value, d.Key);
    //                    }
    //                }
    //                    break;
    //                default: {
    //                    var t = cmd.split(':')[0];//1:voice 2:chat
    //                    var d = utils.getAgentStatus(cmd.split(':')[1]);//1,2,3,4,5,6,7,8
    //                    if (t == '1') {
    //                        if (d.Value == AgentStatus.就绪) {
    //                            SoftPhone.AgentReady();
    //                        }
    //                        else {
    //                            SoftPhone.AgentNotReady(d.Value, d.Key, false);
    //                        }
    //                    }
    //                    else if (t == '2') {
    //                        if (d.Value == AgentStatus.就绪) {
    //                            SoftPhone.CancelNotReadyForMedia_ISvr();
    //                        }
    //                        else {
    //                            SoftPhone.NotReadyForMedia_ISvr(d.Value, d.Key);
    //                        }
    //                    }
    //                }
    //                    break;
    //            }
    //            $("#menu").toggle();
    //        }
    //    }
    //});
    //var TMP_MENU_mouseover = false;
    //$('#menu')
    //    .mouseenter(function () { TMP_MENU_mouseover = true; })
    //    .mouseout(function () { TMP_MENU_mouseover = false; })
    //    .mouseleave(function () {
    //        setTimeout(function () {
    //            if (!TMP_MENU_mouseover) {
    //                var isOpen = $('#menu').css('display');
    //                if (isOpen == 'block') {
    //                    $('#menu').hide();
    //                }
    //            }
    //        }, 100);
    //    });
});

//初始化v2菜单
$(function () {
    $('#v2_menu').width(120);
    $('#v2_menu').menu({
        role: 'menuitem',
        select: function (event, ui) {
            if ($('a:first', ui.item).hasClass('click')) {
                var cmd = $('a:first', ui.item).attr('jtag');
                var t = cmd.split(':')[0];
                if (t == 'all') {
                    var d = utils.getAgentStatus(cmd.split(':')[1]);
                    var isSWtime = false;//是否需要开始记时
                    if (d.Value == '0') {
                        if (cfg.TServerConfig.EnableVoice && !VOICE_Invalid) {
                            if (utils.lenovo_isOn('#v2_lenovo_voice_onoff')) {
                                AgentStatusClick_Voice = d.Key;
                                SoftPhone.AgentReady();
                                isSWtime = true;
                            }
                        }
                        if (cfg.IServerConfig.EnableChat) {
                            if (utils.lenovo_isOn('#v2_lenovo_chat_onoff')) {
                                AgentStatusClick_Chat = d.Key;
                                SoftPhone.CancelNotReadyForMedia_ISvr();
                                isSWtime = true;
                            }
                        }
                    }
                    else {
                        if (cfg.TServerConfig.EnableVoice && !VOICE_Invalid) {
                            if (utils.lenovo_isOn('#v2_lenovo_voice_onoff')) {
                                AgentStatusClick_Voice = d.Key;
                                SoftPhone.AgentNotReady(d.Value, d.Key, false);
                                isSWtime = true;
                            }
                        }
                        if (cfg.IServerConfig.EnableChat) {
                            if (utils.lenovo_isOn('#v2_lenovo_chat_onoff')) {
                                AgentStatusClick_Chat = d.Key;
                                SoftPhone.NotReadyForMedia_ISvr(d.Value, d.Key);
                                isSWtime = true;
                            }
                        }
                    }
                    //开始记时
                    if (isSWtime) {
                        LogMessage('切换记时(138)：' + d.Key);
                    }
                }
                $("#v2_menu").toggle();
            }
        }
    });
    $('#v2_lenovo_voice_onoff').click(function () {
        //按钮自己点击的时候，如果是选中，点击后->checked应该为false
        IsVoiceOnOffClick = true;
        var val = $(this).prop('checked');
        LogMessage('val:' + val);
        if (val) {
            //就绪
            if (cfg.TServerConfig.EnableVoice && !VOICE_Invalid) {
                SoftPhone.AgentReady();
            }
        }
        else {
            //未就绪
            if (cfg.TServerConfig.EnableVoice && !VOICE_Invalid) {
                SoftPhone.AgentNotReadyDefault();
            }
        }
        return false;
    });
    $('#v2_lenovo_chat_onoff').click(function () {
        //按钮自己点击的时候，如果是选中，点击后->checked应该为false
        IsChatOnOffClick = true;
        var val = $(this).prop('checked');
        if (val) {
            //就绪
            if (cfg.IServerConfig.EnableChat) {
                SoftPhone.CancelNotReadyForMedia_ISvr();
            }
        }
        else {
            //未就绪
            if (cfg.IServerConfig.EnableChat) {
                SoftPhone.NotReadyForMedia_ISvr('-1', '未就绪');
            }
        }
        return false;
    });
    var TMP_MENU_mouseover_v2 = false;
    $('#v2_menu')
        .mouseenter(function () { TMP_MENU_mouseover_v2 = true; })
        .mouseout(function () { TMP_MENU_mouseover_v2 = false; })
        .mouseleave(function () {
            setTimeout(function () {
                if (!TMP_MENU_mouseover_v2) {
                    var isOpen = $('#v2_menu').css('display');
                    if (isOpen == 'block') {
                        $('#v2_menu').hide();
                    }
                }
            }, 100);
        });
});

//退出电话条
function LogoutSoftphone() {
    $.blockUI({ message: '正在注销...' });
    LogMessage('退出电话条');

    try {
        lenovoTrace("LogoutSoftphone:" + EmployeeID + "," + AgentIP);
    }
    catch (e) { }

    try {
        //记录退出
        xmlHttpRequest(AppServer_DbUri + 'Db/Logout?EmployeeID=' + EmployeeID + '&t=' + new Date().getTime(), {}, false, function () { });
    }
    catch (e) { }
    var isExit = SoftPhone.Exit();
    if (isExit) {
        if (IS_Lenovo_Business) {
            lenovo_operationDispose(10);//业务退出
        }
    }
    else {
        alert('注销失败');
        $.unblockUI();
        return false;
    };
}

//初始化按钮事件
$(function () {

    $('#Logout').click(function () {
        //window.opener = null;
        //window.open('', '_self')
        //window.close();
        top.window.location.replace('/');
    });

    //window.onbeforeunload = function () {
    //    if (!Is_SphoneToolBar_Unload) {
    //        Is_SphoneToolBar_Unload = true;
    //        LogoutSoftphone();
    //    }
    //}

    window.onbeforeunload = function () {
        if (cfg.TServerConfig.AgentStatus == AgentStatus.处理电话) {
            return '正在通话中，是否关闭？';
        }
        else {
            return '';
        }
    }

    window.onunload = function () {
        if (!Is_SphoneToolBar_Unload) {
            Is_SphoneToolBar_Unload = true;
            LogoutSoftphone();

            try {
                if (chatWin != null) {
                    chatWin.close();
                }
            }
            catch (e) {
            }
        }
    }

    var TMP_dialplate_mouseover = false;
    $("#dialplate").dialog({
        modal: false, draggable: true, resizable: false, closeOnEscape: true, height: 208, width: 242, autoOpen: false,

        position: { my: 'center+28 top+20', at: 'left bottom', of: '#keypad' }
    });
    //$('[aria-describedby="dialplate"]')
    //    .mouseenter(function () {
    //        TMP_dialplate_mouseover = true;
    //    })
    //    .mouseout(function () {
    //        TMP_dialplate_mouseover = false;
    //    })
    //    .mouseleave(function () {
    //        setTimeout(function () {
    //            if (!TMP_dialplate_mouseover) {
    //                var isOpen = $('#dialplate').dialog('isOpen');
    //                if (isOpen) {
    //                    $('#dialplate').dialog('close')
    //                }
    //            }
    //        }, 100);
    //    });

    //状态
    $(document).on('click', '#agentstatus span:first', function () {
        if ($(this).attr('class') != 'disabled') {
            var top = $(this).position().top + 48;
            var left = $(this).position().left - 20;
            $("#v2_menu").css('position', 'absolute').css('top', top).css('left', left);
            $("#v2_menu").toggle();
        }
    });

    //拨号盘
    $(document).on('click', '#keypad .enable', function () {
        var isOpen = $('#dialplate').dialog('isOpen');
        if (isOpen) {
            if ($('div[aria-describedby="dialplate"]').css('display') != 'block') {
                $('div[aria-describedby="dialplate"]').show();
                isOpen = false;
            }
        }
        if (!isOpen) {
            $('#dialplate').dialog('open');
            var val = $('#dialplate .phonenumber').val();

            $('#dialplate .phonenumber').val(val);
        }
        else {
            $('#dialplate').dialog('close');
        }
    });
    $('.phonenumber').keyup(function (e) {
        if (e.keyCode == 13) {
            if ($('#answer span:first').attr('class') == 'MakeCall') {
                if (getConnection(1) == undefined) {
                    $('#answer span:first').click();
                }
                else {
                    $.growlUI('SoftPhone', '请先结束通话');
                }
            }
        }
    });

    //外拨 挂机(挂0线)
    $(document).on('click', '#answer .MakeCall', function () {
        var otherDN = $('.phonenumber').val();
        if (otherDN) {
            if (getConnection(1) == undefined) {
                SoftPhone.MakeCall(utils.lenovo_getRightPhoneNumber(otherDN));
            }
            else {
                $.growlUI('SoftPhone', '请先结束通话');
            }
        }
        else {
            $('#ani .ani').focus();
        }
    });
    $(document).on('click', '#answer .ReleaseCall', function () {
        var connID = getConnection(0);
        if (connID) {
            SoftPhone.ReleaseCall(connID);
        }
    });
    $(document).on('click', '#answer .Dialing', function () {
        var connID = getConnection(0);
        if (connID) {
            SoftPhone.ReleaseCall(connID);
        }
    });

    //保持 取消保持
    $(document).on('click', '#hold .HoldCall', function () {
        var connID = getConnection(0);
        if (connID) {
            SoftPhone.HoldCall(connID);
        }
    });
    $(document).on('click', '#hold .RetrieveCall', function () {
        var connID = getConnection(0);
        if (connID) {
            SoftPhone.RetrieveCall(connID);
        }
    });

    //转接 确认转接 取消转接[1线]
    $(document).on('click', '#transfer .InitiateTransfer', function () {
        var connID = getConnection(0);
        var otherDN = $('.phonenumber').val();
        if (connID && otherDN != '') {
            SoftPhone.UpdateUserData(connID, [
                utils.keyValueToJson('client_app_scenario', 'transfer'),
                utils.keyValueToJson('LenovoConferenceReason', '2')
            ]);
            SoftPhone.InitiateTransfer(connID, utils.lenovo_getRightPhoneNumber(otherDN));
        }
    });
    $(document).on('click', '#transfer .CompleteTransfer', function () {
        var connID = getConnection(0);
        var transferConnID = getConnection(1);
        if (connID && transferConnID) {
            SoftPhone.UpdateUserData(connID, [utils.keyValueToJson('client_app_scenario', '')]);
            SoftPhone.CompleteTransfer(connID, transferConnID);
        }
    });
    $(document).on('click', '#transferCancel .ReleaseCall', function () {
        var connID1 = getConnection(1);
        if (connID1) {
            SoftPhone.ReleaseCall(connID1);
            var connID = getConnection(0);
            if (connID) {
                SoftPhone.RetrieveCall(connID);
            }
        }
    });

    //邀请会议 确认会议 取消会议
    $(document).on('click', '#conference .InitiateConference', function () {
        var connID = getConnection(0);
        var otherDN = $('.phonenumber').val();
        if (connID && otherDN != '') {
            SoftPhone.UpdateUserData(connID, [
                utils.keyValueToJson('LenovoConferenceReason', '3'),
                utils.keyValueToJson('client_app_scenario', 'conference')
            ]);
            SoftPhone.InitiateConference(connID, utils.lenovo_getRightPhoneNumber(otherDN));
        }
    });
    $(document).on('click', '#conference .CompleteConference', function () {
        var connID = getConnection(0);
        var conferenceConnID = getConnection(1);
        if (connID && conferenceConnID) {
            SoftPhone.UpdateUserData(connID, [utils.keyValueToJson('client_app_scenario', '')]);
            SoftPhone.CompleteConference(connID, conferenceConnID);
            vf.IsConference = 1;
            SoftPhone.VoiceFlowUpdate(vf);
        }
    });
    $(document).on('click', '#conferenceCancel .ReleaseCall', function () {
        var connID1 = getConnection(1);
        if (connID1) {
            SoftPhone.ReleaseCall(connID1);
            var connID = getConnection(0);
            if (connID) {
                SoftPhone.RetrieveCall(connID);
            }
        }
    });

    //chat 不需要，提示3秒即可。

    // 转接 电话支付 转满意度
    $(document).on('click', '#transferSearch .enable', function () {
        var connID = getConnection(0);
        if (connID) {

            var retValue = window.showModalDialog('SoftPhoneToolBar/TransferSearch', window, 'dialogWidth:580px,dialogHeight:620px');
            if (retValue) {
                var arr = retValue.split(":");
                if ('agent' == arr[0]) {
                    $('.phonenumber').val('#' + arr[1]);
                    SoftPhone.UpdateUserData(connID, [
                        utils.keyValueToJson('LenovoConferenceReason', '2'),
                        utils.keyValueToJson('strPreAgentgroupName', vf.AgentgroupName),
                        utils.keyValueToJson('strCustomerID', utils.getUserDataValue(vf.UserData, 'strCustomerID')),
                        utils.keyValueToJson('CenterName', cfg.CallCenter),
                        utils.keyValueToJson('client_app_scenario', '')
                    ]);
                    vf.IsTransfer = 2;
                    SoftPhone.VoiceFlowUpdate(vf);
                    $('#transfer .InitiateTransfer').click();
                }
                else if ('agentGroup' == arr[0]) {
                    SoftPhone.UpdateUserData(connID, [
                        utils.keyValueToJson('LenovoConferenceReason', '1'),
                        utils.keyValueToJson('strPreAgentgroupName', vf.AgentgroupName),
                        utils.keyValueToJson('strCustomerID', utils.getUserDataValue(vf.UserData, 'strCustomerID')),
                        utils.keyValueToJson('CenterName', cfg.CallCenter),
                        utils.keyValueToJson('client_app_scenario', ''),

                        utils.keyValueToJson('Function', '998'),
                        utils.keyValueToJson('strPreCallID', vf.CallID),
                        utils.keyValueToJson('strNextAgentgroupName', arr[1]),
                        utils.keyValueToJson('lenovo_transferTo', 'group')
                    ]);
                    var otherDN = config_TRANSFERDN.转队列;
                    if ('SIP' == VOICE_SUPPORT_TYPE) {
                        SoftPhone.InitiateTransfer(connID, otherDN);
                        setTimeout(function () {
                            SoftPhone.CompleteTransfer(connID, getConnection(1));
                        }, 500);
                    }
                    else {
                        SoftPhone.MuteTransfer(connID, otherDN);
                    }
                    vf.NextAgentgroupName = arr[1];
                    vf.IsTransfer = 1;
                    SoftPhone.VoiceFlowUpdate(vf);
                }
            }
        }
    });
    $(document).on('click', '#transferTelPay .enable', function () {
        var connID = getConnection(0);
        var otherDN = config_TRANSFERDN.电话支付;
        if (connID && otherDN != '') {
            SoftPhone.UpdateUserData(connID, [utils.keyValueToJson('client_app_scenario', 'telpay')]);
            SoftPhone.InitiateConference(connID, otherDN);
            vf.IsTransferEPOS = 1;
            SoftPhone.VoiceFlowUpdate(vf);
        }
    });
    $(document).on('click', '#ivr .enable', function () {
        var connID = getConnection(0);
        var otherDN = config_TRANSFERDN.转满意度;
        if (connID && otherDN != '') {
            SoftPhone.UpdateUserData(connID, [
                utils.keyValueToJson('Function', '999'),
                utils.keyValueToJson('CALLID', vf.CallID),
                utils.keyValueToJson('AgentID', EmployeeID)
            ]);
            SoftPhone.MuteTransfer(connID, otherDN);
            vf.IsTransfer = 3;
            SoftPhone.VoiceFlowUpdate(vf);
        }
    });
});

//小键盘初始化
$(function () {
    $('#dialplate table td a').click(function () {
        var str = $('.phonenumber').val();
        $('.phonenumber').val(str + $(this).text());
        $(this).blur();
        $('.phonenumber').focus();
        $(this).blur();
        return false;
    });

    $('#dialplate .backspace').click(function () {
        var str = $('.phonenumber').val();
        var result = '';
        if (str.length > 0) {
            result = str.substr(0, str.length - 1);
        }
        $('.phonenumber').val(result);
        $(this).blur();
        $('.phonenumber').focus();
        $(this).blur();
        return false;
    });
});

function agentOpenStatistic(url, person) {
    /// <summary>
    /// 打开订阅
    /// </summary>
    /// <param name="url"></param>
    /// <param name="person"></param>
    var data = { person: JSON.stringify(person) };
    $.getJSON(url, data, function (r) {
        if (r.Code == -1) {
            //错误
            setTimeout(function () { agentOpenStatistic(url, person); }, 3000);
        }
        else {
            clearTimeout(Interval_000);
        }
    }).error(function () {
        setTimeout(function () { agentOpenStatistic(url, person); }, 3000);
    });
}

function agentGetStatistic(url, personDBID) {
    /// <summary>
    /// 查找订阅结果
    /// </summary>
    /// <param name="url"></param>
    /// <param name="personDBID"></param>
    var data = { personDBID: personDBID };
    $.getJSON(url, data, function (r) {
        if (r.Code == -1) {

        }
        else {
            $('#QueueCount1').html(r.d.QueueCount1);
            $('#QueueCount2').html(r.d.QueueCount2);
            $('#CallInCount').html(r.d.CallInCount);
            $('#CallOutCount').html(r.d.CallOutCount);
            $('#ChatInCount').html(r.d.ChatInCount);

            $('#time2').val(utils.secondFormat(r.d.AHT));
        }
        setTimeout(function () { agentGetStatistic(url, personDBID); }, 3000);
    }).error(function () {
        setTimeout(function () { agentGetStatistic(url, personDBID); }, 3000);
    });
}

//处理坐席状态更改,2013.07.03,默认异步,1电话 2文本
function processAgentStatus(type, currentAgentStatus, isSync) {
    if (isSync == undefined) {
        isSync = true;
    }
    if (type == 1) {
        if (currentAgentStatus == '后续跟进') {
            if (changeLastStatus1 != '后续跟进') {
                changeLastLogID1 = EmployeeID + GetCurrentTime() + 'V';
                //insert  
                var url = AppServer_DbUri + 'Db/ProcessAgentStatus?callback=?';
                var data = {
                    LogID: changeLastLogID1,
                    EmployeeID: EmployeeID,
                    TypeID: 1,
                    InsertOrUpdate: 0
                };
                if (isSync) {
                    $.getJSON(url, data, function (r) { });
                }
                else {
                    xmlHttpRequest(url, data, false, function (r) { });
                }
            }
            changeLastStatus1 = currentAgentStatus;

        }
        else {
            if (changeLastStatus1 == '后续跟进') {
                //update
                var url = AppServer_DbUri + 'Db/ProcessAgentStatus?callback=?';
                var data = {
                    LogID: changeLastLogID1,
                    EmployeeID: EmployeeID,
                    TypeID: 1,
                    InsertOrUpdate: 1
                };
                if (isSync) {
                    $.getJSON(url, data, function (r) { });
                }
                else {
                    xmlHttpRequest(url, data, false, function (r) { });
                }
            }
            changeLastStatus1 = currentAgentStatus;
        }
    }
    else {
        if (currentAgentStatus == '后续跟进') {
            if (changeLastStatus2 != '后续跟进') {
                changeLastLogID2 = EmployeeID + GetCurrentTime() + 'C';
                //insert  
                var url = AppServer_DbUri + 'Db/ProcessAgentStatus?callback=?';
                var data = {
                    LogID: changeLastLogID2,
                    EmployeeID: EmployeeID,
                    TypeID: 2,
                    InsertOrUpdate: 0
                };
                if (isSync) {
                    $.getJSON(url, data, function (r) { });
                }
                else {
                    xmlHttpRequest(url, data, false, function (r) { });
                }
            }
            changeLastStatus2 = currentAgentStatus;

        }
        else {
            if (changeLastStatus2 == '后续跟进') {
                //update
                var url = AppServer_DbUri + 'Db/ProcessAgentStatus?callback=?';
                var data = {
                    LogID: changeLastLogID2,
                    EmployeeID: EmployeeID,
                    TypeID: 2,
                    InsertOrUpdate: 1
                };
                if (isSync) {
                    $.getJSON(url, data, function (r) { });
                }
                else {
                    xmlHttpRequest(url, data, false, function (r) { });
                }
            }
            changeLastStatus2 = currentAgentStatus;
        }
    }
}

//打开聊天窗口
var chatWin = null, isEnable = false;
function setChatStatus(status) { isEnable = status; }
function openChatWin(ticketId, interactionId, userData) {
    function waitForChat() {
        if (!isEnable) setTimeout(waitForChat, 200);
        else {
            try {
                chatWin.eventAccepted(ticketId, interactionId, data);// chat/chat_api.js
            }
            catch (e) {
                chatWin = null; isEnable = false;
                openChatWin(ticketId, interactionId, data);
            }
        }
    }
    var data = userData;
    if (typeof data == 'object') {
        data = JSON.stringify(data);
    }
    if (chatWin == null) {
        var left = (window.screen.availWidth - 800) / 2; //获得窗口的水平位置;
        var top = (window.screen.availHeight - 600) / 2; //获得窗口的垂直位置;
        chatWin = window.open('/Chat/Index?_t=' + new Date().getTime(), '_chatWin', 'height=660,width=600,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no,left=' + left + ',top=' + top + '');
    }
    setTimeout(waitForChat, 200);
}