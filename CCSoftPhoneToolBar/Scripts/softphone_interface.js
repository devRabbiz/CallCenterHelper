//软电话OCX
var SPhone = null;
//业务系统
var BusinessAppUrl = '';

//数据访问的地址，如:http://domain.com/ 以/结尾,服务访问
var AppSoftPhoneServiceUri = '';

//数据访问
var AppServer_DbUri = '';

//检测是否订阅成功
var Interval_000 = -1;
//电话登录使用
var Interval_100 = -1;
//检测是否登录成功
var Interval_101 = -1;
//记时
var Interval_102 = -1;
//记时
var Interval_102_Reset = -1;

//切换其他状态时开始记时
var Interval_103 = -1;

var IsVoiceOnOffClick = false;
var IsChatOnOffClick = false;
//点了v2_menu的哪个状态,
var AgentStatusClick_Chat = '';
//点了v2_menu的哪个状态
var AgentStatusClick_Voice = '';



//chat登录使用
var Interval_200 = -1;

//临时用，为了写js有提示
var OCXAutoJsonMerge = false;

//员工编号
var EmployeeID = '';

//员工DBID
var PERSON_DBID = 0;

//坐席状态
var AgentStatus = {
    '未就绪': -1, '就绪': 0, '处理电话': 1, '案头工作': 2, '后续跟进': 3, '开会': 4, '培训': 5, '休息': 6, '午餐': 7, '其他': 8
}

//呼叫类型
var CallType = {
    Unknown: 0, Internal: 1, Inbound: 2, Outbound: 3, Consult: 4
}

//默认配置json
var __CFG = {
    //'FromBJ','FromNJ'
    CallCenter: '',
    //是否显示OCX 日志Info消息
    LogMessage: false,
    //是否显示OCX 日志Response消息
    LogResponse: false,
    //是否显示OCX 日志Request消息
    LogRequest: false,
    //是否记录跟踪
    LogTrace: false,
    //配置
    TServerConfig: {
        EnableVoice: false,
        TSERVER_IDENTIFIER: '',
        TSERVER_URI: '',
        TSERVER_CLIENT_NAME: '',
        TSERVER_CLIENT_PASSWORD: '',
        TSERVER_QUEUE: '',
        DN: '',
        AgentID: '',
        Password: '',
        AgentStatus: 0,

        IsLinkConnected: false,
        IsLinkDisconnected: false,

        IsRegistered: false,
        IsUnregistered: false,

        IsAgentLogin: false,
        IsAgentLogout: false,

        Exit: false,

        HASupport: false,
        Bakup_TSERVER_URI: ''

    },
    IServerConfig: {
        EnableChat: false,
        ISERVER_IDENTIFIER: '',
        ISERVER_URI: '',
        ISERVER_CLIENT_NAME: '',
        TenantId: 101,
        AgentID: '',
        PlaceID: '',
        AgentStatus: 0,

        IsLinkConnected: false,
        IsAgentLogin: false,
        IsAgentLogout: false,

        Exit: false,
        WarmStandby: false,
        HASupport: false,
        Bakup_ISERVER_URI: ''
    }
}

//默认话务流json
var __VF = {
    //员工编号+yyyyMMddHHmmss
    CallID: '',
    //[否决的]客户编号 => 用随路数据
    CustomerID: '',
    //当前电话的InteractionID
    InteractionID: '',
    //当前坐席状态
    AgentStatus: 0,
    //当前通话类型:Unknown 未知,Internal 内线,Inbound 呼入,Outbound 外拨,Consult 咨询
    CallType: 0,
    //当前电话的0线 ConnectionID
    ConnectionID: '',
    //哪个connid触发的
    CurrentConnectionID: '',
    //mute过来的就是这个,非mute接起CurrentConnectionID
    TransferConnectionID: '',
    //当前电话的1线 ConnectionID
    SecondConnectionID: '',
    //电话来自的队列
    AgentgroupName: '',
    //前一个坐席组
    PreAgentgroupName: '',
    //如果要转接出去的话，代表后一个坐席组
    NextAgentgroupName: '',
    //转接过来的话，获得前一个坐席
    PreAgentId: '',
    //转接出去的话，代表下一个坐席
    NextAgentId: '',
    //主叫号码
    ANI: '',
    //被叫号码
    DNIS: '',
    //哪个分机号参与了这一请求或事件
    ThisDN: '',
    //随路数据
    UserData: {},
    //[否决的]当前队列名称
    CurrentQueueName: '',
    //[否决的]转入队列名称
    FromQueueName: '',
    //[否决的]转出队列名称
    NextQueueName: '',
    //1：会议
    IsConference: 0,
    //0：默认没有转接 1：转接至队列 2：转接至坐席 3：转接至IVR 
    IsTransfer: 0,
    //1：转接至电话支付
    IsTransferEPOS: 0,
    //1:呼入 2：预览呼出 3：自动呼出
    InOut: 0,
    //电话支付下：第三方的DN。如果包含在ANI或DNIS里能找到则说明用户挂断。（提示坐席，并且挂断0线）
    ThirdPartyDN: '',
    //Internal，需要显示主叫坐席分机
    OtherDN: ''
}


//外拨 接通 0线是否结束
var SF_Call0IsEnd = false;

//案头工作:等待切换到非案头工作
var SF_WaitingDeskTime_CallID = '';

//是否登录了业务系统
var IS_Lenovo_Business = false;

//是否已经卸载了电话条
var Is_SphoneToolBar_Unload = false;

//是否是无效的语音(没有真实话机):true为没有话机
var VOICE_Invalid = false;

//是否是无效的chat(一直登录不上)
var Chat_Invalid = false;

//ws主备切换
var Chat_Ws_ReLogin = false;

//chat最后坐席状态
var Chat_LastAgentState = { code: '', description: '' }

//坐席IP
var AgentIP = '';

//Open统计服务集：多个用英文逗号分割
var NLBOpenStaticAppServers = '';

//电话条使用类型
var ToolBarUseType = { 'LOCAL': 0, 'SDI': 1, 'ESD': 2, 'Unknow': -1 };
//0.应急 1.sdi 2.esd
var ToolBarSet = ToolBarUseType.Unknow;

//状态下拉、拨号盘需要用到
var topJ = $;

//是否非正常退出,通过话机硬退出。（被别的电话条踢下来）
var AgentLogout_AbNormal = false;

//是否收到620错误，登录成功后清除
var ReLoginError = false;

//是否是CC测试
var IsCCNowTest = false;

//报工号时的控件变量
var sayempno = {
    //是否在报工号
    IsSayempno: false,
    //是否取回通话
    IsRetrieved: false
}

//2013.07.03

/*后续跟进用.begin*/
//voice坐席最后状态
var changeLastStatus1 = '就绪';
//voice的logid
var changeLastLogID1 = '';

//chat坐席最后状态
var changeLastStatus2 = '就绪';
//chat的logid
var changeLastLogID2 = '';
/*后续跟进用.end*/


//预约回拨配置 2014.12.29
var reserveCallBack = {
    //外拨ID
    callBackID: '',
    //电话号码
    phoneNumber: '',
    //正在当前外呼队列
    isInCurrent: true,
    //正在推送中
    isPushing:false
}