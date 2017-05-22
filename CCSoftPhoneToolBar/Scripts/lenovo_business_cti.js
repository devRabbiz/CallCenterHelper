/// <reference path="softphone_interface.js" />
/// <reference path="softphone_config.js" />
/// <reference path="softphone_api.js" />

var lenovo_rote_data = {
    //------------------------
    'lUserId': '',//员工编号
    'strLoginId': '',//AgentLogin.LoginCode
    'lVoiceDNCTIId': '',//分机号DBID，不需要了
    //-------------------------
    'strCurCallID': 'CallID',//-----callID
    'strANI': 'ANI',//主叫号码
    'strRegisterNumber': 'strRegisterNumber',//为空取strANI
    'strDNIS': 'DNIS',//被叫号码
    'strCalltype': '',//1,2,3
    'interactionid': 'ConnectID',
    'strAgentgroupName': 'VirualQueue',//------当前队列
    'strPreAgentgroupName': 'strPreAgentgroupName',
    'strPreAgentId': 'strPreAgentId',
    'strNextAgentgroupName': 'strNextAgentgroupName',//-------转出的队列
    'strNextAgentId': 'strNextAgentId',
    'strSatisfy': '',//是否转满意度,给空
    'strPreCallID': 'strPreCallID',
    'strCustomerID': 'strCustomerID',
    'strFinished': 'strFinished',//字符:true,false
    'CCCaseID': 'CCCaseID',
    'GSW_SampleID': 'SampleID',
    'GSW_ActivityID': 'ActivityID',
    'strReserveCallBackID': 'strReserveCallBackID',//预约回拨ID
    'strCallbackAuto': 'strCallbackAuto',//自动预约外拨为1
    //---------------------------------
    'strServiceCardNo': '',
    //--------------------------------- chat
    'strSdiUserId': '',
    'strCurCallIDChat': '',
    'strRegisterNumberChat': '',
    'interactionidChat': '',
    'strAgentgroupNameChat': '',
    'strPreAgentgroupNameChat': '',
    'strServiceCardNoChat': '',
    'strPreCallIDChat': '',
    'strNextAgentgroupNameChat': '',
    'strBeginTimeChat': '',
    'strFinishTimeChat': '',
    //-----------------------------------
    'strBusinessCode': '',
    'strBusinessParam': ''

}

function lenovo_operationDispose(stateflag) {
    /// <summary>
    /// 1:login;
    /// 2:taken:queue in 
    /// 3:taken:Trans queue in
    /// 4:taken:Trans agent in
    /// 5:end call:to agent or queue
    /// 6:end call:to IVR
    /// 7:end call:finish call
    /// 8:ready
    /// 9:notready
    /// 10:logout
    /// 11:outbound
    /// 13:自动外拨
    /// 
    /// 14:回拨正在拨打.2015.01.06
    /// </summary>
    /// <param name="stateflag">int</param>
    try {
        if (top != self && top.sdiTelCall && IS_Lenovo_Business) {
            top.sdiTelCall(stateflag);
            LogMessage('top.sdiTelCall(' + stateflag + ')');
        }
        else {
            LogMessage('top.sdiTelCall(' + stateflag + ')');
            LogMessage('top.sdiTelCall=>未调用成功');
        }
    }
    catch (e) {
        alert('sdiTelCall error:' + e);
    }
}

/**
  * get the value according to the key specified in attached data
  * associated with current call
  * @param key the specifical key
  * @return the value
  * mediaType : 1 email    2 chat   0,null is voice
  */
function getAttachedDataValue(key, mediaType) {
    var val = utils.getUserDataValue(vf.UserData, key);
    return val;
}

/**
  * add attached data into current call
  * @param keyArray keys array
  * @param valueArray values array
  *     NOTE: the keys must exactly match the values
  * @return true/false  succeed or not
  * mediaType : 1 email    2 chat   0,null is voice
  */
function addAttachedData(keyArray, valueArray, mediaType) {
    if (!SPhone) {
        alert("Softphone activex not found!");
        return;
    }

    var _listkv = [];
    for (var i = 0; i < keyArray.length; i++) {
        var kv = [utils.keyValueToJson(keyArray[i], valueArray[i])];
        $.extend(_listkv, kv);
    }

    if (mediaType == null || mediaType == 'undifined' || mediaType == '0') {
        var connID = getConnection(0);
        if (connID) {
            SoftPhone.UpdateUserData(connID, _listkv);
        }
    }
    else {
        alert('未实现');
    }
}

function lenovo_trimSkillGroup(s) {
    if (s.substring(0, 1).toUpperCase() == "V")
        s = s.substring(1);
    else
        s = s;
    return s;
}

function getPropertyValue(key) {

    if ($.inArray(key, [
        'strSdiUserId',
        'strCurCallIDChat',
        'strRegisterNumberChat',
        'interactionidChat',
        'strAgentgroupNameChat',
        'strPreAgentgroupNameChat',
        'strServiceCardNoChat',
        'strPreCallIDChat',
        'strNextAgentgroupNameChat',
        'strBeginTimeChat',
        'strFinishTimeChat']) != -1) {
        return lenovo_rote_data[key];
    }

    if (key == 'strLoginId') {
        return EmployeeID;
    }
    if (key == 'lUserId' || key == 'strLoginId') {
        return lenovo_rote_data[key];
    }
    var newKey = lenovo_rote_data[key];
    if (newKey == undefined) {
        return '';
    }
    if (newKey == '') {
        newKey = key;
    }
    if (newKey == 'CallID') {
        val = vf.CallID;
        if (!val) {
            val = '';
        }
        return val;
    }
    var val = getAttachedDataValue(newKey);
    if (key == 'strRegisterNumber') {
        if (val == '' || val == '0' || val.length <= 3) {
            return getPropertyValue('strANI');
        }
    }
    else if (key == 'strAgentgroupName') {
        return lenovo_trimSkillGroup(val);
    }
    else if (key == 'strDNIS') {
        return utils.lenovo_getRightDNIS(val);
    }
    return val;
}

//业务会调用
function logoutProduct() {
    if (cfg.TServerConfig.AgentStatus == AgentStatus.处理电话) {
        $.growlUI('SoftPhone Notification', '通话中无法注销');
        return false;
    }
    else {
        window.location.replace('/Home/Login?popup=1');
    }
}

// 确认用户记录Call Info
function lenovo_confirmUser(callId, userID) {

}

function makeNewCall(phoneNumber, lineId, keyArray, valueArray) {
    if (!SPhone) {
        alert("Softphone activex not found!");
        return;
    }
    if (AgentLogout_AbNormal) {
        alert('话机不可用');
        return;
    }
    var _listkv = [];
    if (keyArray && typeof keyArray == 'object') {
        for (var i = 0; i < keyArray.length; i++) {
            var kv = utils.keyValueToJson(keyArray[i], valueArray[i]);
            _listkv.push(kv);
        }
    }

    _listkv.push(utils.keyValueToJson('CallCenter', cfg.CallCenter));

    if ($('#answer span:first').attr('class') == 'MakeCall') {
        if (getConnection(1) == undefined) {
            SoftPhone.MakeCall(utils.lenovo_getRightPhoneNumber(phoneNumber), _listkv);
        }
        else {
            $.growlUI('SoftPhone', '请先结束通话');
        }
    }
}

function makeNewCall_ESD(phoneNumber, customerID) {
    /// <summary>
    /// ESD用：外拨
    /// </summary>
    /// <param name="phoneNumber">电话号码</param>
    /// <param name="customerID">客户ID,如无法确认客户ID给空或null</param>
    if (customerID && customerID != '') {
        makeNewCall(phoneNumber, '0', new Array('strCustomerID'), new Array(customerID + ''));
    }
    else {
        makeNewCall(phoneNumber);
    }
}

function releaseCall_ESD() {
    /// <summary>
    /// ESD用：结束通话
    /// </summary>
    if (!SPhone) {
        alert("Softphone activex not found!");
        return;
    }
    if (AgentLogout_AbNormal) {
        alert('话机不可用');
        return;
    }
    var connID = getConnection(0);
    if (connID) {
        SoftPhone.ReleaseCall(connID);
    }
}

function get_lenovo_userid(id) {
    //lenovo_userid = id;
    //新系统不需要
}

function lenovo_endBusiness(callid) {
    //结束业务时
}


//是否加载了电话条:1是加了，0未加载
function TelLoad(i, msg) {
    if (ToolBarSet == ToolBarUseType.ESD) {
        AgentLogout_AbNormal = (i == 0);
        return;
    }
    if (top != self && top.TelLoad && IS_Lenovo_Business) {
        top.TelLoad(i, msg);
    }
    else {
        if (i == 0) {
            alert(msg);
        }
    }
}

//#region 回拨 2015.01.08

function getCallBackPushEnable() {
    /// <summary>
    /// ESD:获取是否可推送,成功返回true
    /// </summary>
    if (reserveCallBack.callBackID == '' && reserveCallBack.isPushing==false) {
        return true;
    }
    return false;
}

function setReserveCallBack(callBackID, phoneNumber) {
    /// <summary>
    /// ESD:设置预约回拨,成功返回true
    /// </summary>
    /// <param name="callBackID">回拨ID</param>
    /// <param name="phoneNumber">电话号码</param>
    if (reserveCallBack.callBackID == '') {
        reserveCallBack.callBackID = callBackID;
        reserveCallBack.phoneNumber = phoneNumber;
        reserveCallBack.isInCurrent = true;
        return true;
    }
    return false;
}

function setPushStatus(b) {
    reserveCallBack.isPushing = b;
}

function prepareReserveCallBack() {
    /// <summary>
    /// ESD:准备预约回拨
    /// </summary>
    //就绪时外拨
    if (reserveCallBack.callBackID != '' && reserveCallBack.phoneNumber != '' && cfg.TServerConfig.AgentStatus == AgentStatus.就绪 && reserveCallBack.isInCurrent) {
        reserveCallBack.isInCurrent = false;
        var cbStatus = top.Tel_CB_Status(reserveCallBack.callBackID);
        if (cbStatus != -1) {
            if (cbStatus == 0) {
                makeNewCall(reserveCallBack.phoneNumber, '0', new Array('strReserveCallBackID', 'strCallbackAuto'), new Array(reserveCallBack.callBackID + '', '1'));
            }
            else {
                reserveCallBack.callBackID = '';
                reserveCallBack.isInCurrent = true;
            }
        }
        else {
            reserveCallBack.callBackID = '';
            reserveCallBack.isInCurrent = true;
        }
    }
    setTimeout(function () { prepareReserveCallBack(); }, 1000);
}

setTimeout(function () { prepareReserveCallBack(); }, 1000);

function makeNewCall_CB(phoneNumber, callbackID) {
    /// <summary>
    /// 预约回拨用：外拨
    /// </summary>
    /// <param name="phoneNumber">电话号码</param>
    /// <param name="callbackID">回拨ID</param>
    if (callbackID && callbackID != '') {
        makeNewCall(phoneNumber, '0', new Array('strReserveCallBackID'), new Array(callbackID + ''));
    }
    else {
        makeNewCall(phoneNumber);
    }
}
//#endregion