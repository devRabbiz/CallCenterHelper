using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using mshtml;

#region TSVR
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Voice.Protocols;
using Genesyslab.Platform.Voice.Protocols.TServer;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
using Genesyslab.Platform.Voice.Protocols.TServer.Requests.Dn;
using Genesyslab.Platform.Voice.Protocols.TServer.Requests.Agent;
using Genesyslab.Platform.Voice.Protocols.TServer.Requests.Party;
using Genesyslab.Platform.Voice.Protocols.TServer.Requests.Userdata;
#endregion

#region ISVR
using ISVR = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
#endregion

namespace VoiceOCX
{

#pragma warning disable 612, 618
    [Guid("F465D5AB-F971-4633-BB45-D226B18422FF")]
    public partial class SoftPhone : UserControl, IObjectSafety
    {
        #region Private Members - For Genesys AppBlocks used (Protocol Manager and Message Broker)
        private ProtocolManagementService protocolManagementService;
        private TServerConfiguration tserverConfiguration;
        private InteractionServerConfiguration iserverConfiguration;
        private EventBrokerService eventBrokerService;
        private int iServerClientID = 0;
        private bool AppExit = false;
        private bool IsPreAppExit = false;
        #endregion

        public SoftPhone()
        {
            InitializeComponent();
            eventLog1.Source = "SoftPhone";
        }

        public string ClientVersion
        {
            get
            {
                return "1.0.3.1";
            }
        }

        #region ********************************************Genesys Event Handlers********************************************
        private void TServerEventsHandler(IMessage response)
        {
            try
            {
                if (IsPreAppExit)
                {
                    return;
                }
                if (this.InvokeRequired)
                {
                    if (!AppExit)
                    {
                        GenesysEventHandlerDelegate handler = new GenesysEventHandlerDelegate(this.TServerEventsHandler);
                        this.Invoke(handler, new object[] { response });
                    }
                }
                else
                {
                    LogResponse(response.ToString());

                    var errorMessage = "";
                    var errorResponse = "";
                    var errorCode = 0;
                    switch (response.Id)
                    {
                        #region 连接,断开连接
                        case EventLinkConnected.MessageId:
                            {
                                var e = (EventLinkConnected)response;
                                cfg.TServerConfig.IsLinkConnected = true;
                            }
                            break;
                        case EventLinkDisconnected.MessageId:
                            {
                                var e = (EventLinkConnected)response;
                                cfg.TServerConfig.IsLinkDisconnected = true;
                            }
                            break;
                        #endregion

                        #region 注册 反注册
                        case EventRegistered.MessageId:
                            {
                                var e = (EventRegistered)response;
                                cfg.TServerConfig.IsRegistered = true;
                            }
                            break;
                        case EventUnregistered.MessageId:
                            {
                                cfg.TServerConfig.IsUnregistered = true;
                            }
                            break;
                        #endregion

                        #region 登录 注销
                        case EventAgentLogin.MessageId:
                            {
                                var e = (EventAgentLogin)response;
                                cfg.TServerConfig.IsAgentLogin = true;
                            }
                            break;
                        case EventAgentLogout.MessageId:
                            {
                                if (cfg.TServerConfig.IsAgentLogin)
                                {
                                    cfg.TServerConfig.IsAgentLogout = true;
                                }
                            }
                            break;
                        #endregion

                        #region 就绪 未就绪
                        case EventAgentReady.MessageId:
                            {
                                vf = new Models.VoiceFlow();
                                var e = (EventAgentReady)response;
                                cfg.TServerConfig.AgentStatus = Models.AgentStatus.就绪;
                            }
                            break;
                        case EventAgentNotReady.MessageId:
                            {
                                var e = (EventAgentNotReady)response;
                                if (e.Reasons != null)
                                {
                                    try
                                    {
                                        if (e.Reasons.ContainsKey("OCXReasonCode"))
                                        {
                                            var value = int.Parse(e.Reasons["OCXReasonCode"].ToString().Split(':')[0]);
                                            cfg.TServerConfig.AgentStatus = (Models.AgentStatus)value;
                                        }
                                    }
                                    catch
                                    {
                                        cfg.TServerConfig.AgentStatus = Models.AgentStatus.未就绪;
                                    }
                                }
                                else
                                {
                                    if (e.AgentWorkMode == AgentWorkMode.AfterCallWork)
                                    {
                                        cfg.TServerConfig.AgentStatus = Models.AgentStatus.案头工作;
                                    }
                                    else
                                    {
                                        cfg.TServerConfig.AgentStatus = Models.AgentStatus.其他;
                                    }
                                }
                            }
                            break;
                        #endregion

                        //本机振铃
                        case EventRinging.MessageId:
                            {
                                if (IsTalking || cfg.TServerConfig.AgentStatus == Models.AgentStatus.处理电话)
                                {
                                    return;
                                }

                                if (IsDialing)
                                {
                                    return;
                                }

                                var e = (EventRinging)response;
                                vf = new Models.VoiceFlow();

                                vf.ConnectionID = e.ConnID.ToString();
                                vf.CurrentConnectionID = e.ConnID.ToString();
                                if (e.TransferConnID != null)
                                {
                                    vf.TransferConnectionID = e.TransferConnID.ToString();
                                }
                                vf.CallType = e.CallType;

                                vf.ANI = e.ANI;
                                vf.DNIS = e.DNIS;
                                vf.ThisDN = e.ThisDN;
                                vf.InOut = 1;
                                vf.OtherDN = e.OtherDN;
                                if (e.UserData != null)
                                {
                                    foreach (System.Collections.DictionaryEntry item in e.UserData)
                                    {
                                        var key = (item.Key ?? "").ToString();
                                        var val = (item.Value ?? "").ToString();
                                        if (!string.IsNullOrEmpty(key))
                                        {
                                            if (vf.UserData.ContainsKey(key))
                                            {
                                                vf.UserData[key] = val;
                                            }
                                            else
                                            {
                                                vf.UserData.Add(key, val);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        //接通
                        case EventEstablished.MessageId:
                            {
                                var e = (EventEstablished)response;
                                vf.CurrentConnectionID = e.ConnID.ToString();
                            }
                            break;

                        //挂断
                        case EventReleased.MessageId:
                            {
                                //用户挂断,主动挂断,转接时挂断
                                var e = (EventReleased)response;
                                vf.CallType = e.CallType;
                                vf.CurrentConnectionID = e.ConnID.ToString();
                                if (e.CallType == CallType.Consult)
                                {

                                }
                            }
                            break;

                        //来电者的电话之前被遗弃了
                        case EventAbandoned.MessageId:
                            {
                                var e = (EventAbandoned)response;
                                vf.CurrentConnectionID = e.ConnID.ToString();
                            }
                            break;

                        //外拨中，如果是咨询类就是1线
                        case EventDialing.MessageId:
                            {
                                var e = (EventDialing)response;
                                if (e.CallType == CallType.Consult)
                                {
                                    vf.SecondConnectionID = e.ConnID.ToString();
                                }
                                else
                                {
                                    vf.ConnectionID = e.ConnID.ToString();
                                }
                                vf.CurrentConnectionID = e.ConnID.ToString();
                                vf.CallType = e.CallType;
                            }
                            break;

                        case EventMuteOn.MessageId:
                            {
                                var e = (EventMuteOn)response;
                            }
                            break;

                        case EventMuteOff.MessageId:
                            {
                                var e = (EventMuteOff)response;
                            }
                            break;

                        //指定的otherdn电话对象已经取代了先前接收到的事件的otherdn指定电话对象；或者打电话的previousconnid已被赋予了新的价值，connid。
                        case EventPartyChanged.MessageId:
                            {
                                var e = (EventPartyChanged)response;
                                //丢弃:因为A B 通话中，B->(mute转队列、确定转接、确定会议) C 那:C A都能收到这个事件
                                vf.CallType = e.CallType;
                                vf.CurrentConnectionID = e.ConnID.ToString();
                                vf.ConnectionID = e.ConnID.ToString();
                            }
                            break;
                        case EventError.MessageId:
                            {
                                var e = (EventError)response;
                                errorResponse = response.ToString();
                                errorMessage = e.ErrorMessage;
                                errorCode = e.ErrorCode;
                                if (string.IsNullOrEmpty(errorMessage))
                                {
                                    errorMessage = response.ToString();
                                    LogException(new Exception("TS-ERR:" + e.ToString()));
                                }
                            }
                            break;

                        //随路数据有变化
                        case EventAttachedDataChanged.MessageId:
                            {
                                var e = (EventAttachedDataChanged)response;
                                if (e.UserData != null)
                                {
                                    foreach (System.Collections.DictionaryEntry item in e.UserData)
                                    {
                                        var key = (item.Key ?? "").ToString();
                                        var val = (item.Value ?? "").ToString();
                                        if (!string.IsNullOrEmpty(key))
                                        {
                                            if (vf.UserData.ContainsKey(key))
                                            {
                                                vf.UserData[key] = val;
                                            }
                                            else
                                            {
                                                vf.UserData.Add(key, val);
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                        case EventPartyDeleted.MessageId:
                            {
                                var e = (EventPartyDeleted)response;
                                vf.ThirdPartyDN = e.ThirdPartyDN;
                            }
                            break;
                    }
                    CallJavaScript("HandlerTServerEvent", response.Name, errorResponse, errorCode, errorMessage);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void IServerEventsHandler(IMessage response)
        {
            try
            {
                LogResponse(response.ToString());
                if (IsPreAppExit)
                {
                    return;
                }
                if (this.InvokeRequired)
                {
                    if (!AppExit)
                    {
                        GenesysEventHandlerDelegate handler = new GenesysEventHandlerDelegate(this.IServerEventsHandler);
                        this.Invoke(handler, new object[] { response });
                    }
                }
                else
                {
                    LogResponse(response.ToString());
                    var errorResponse = string.Empty;
                    var errorCode = 0;
                    var errorMessage = string.Empty;
                    var interactionId = string.Empty;
                    switch (response.Id)
                    {
                        case ISVR.Events.EventAck.MessageId:
                            {
                                //有事件响应
                                var e = (ISVR.Events.EventAck)response;
                                var ackType = Models.AckType.未知;
                                try
                                {
                                    ackType = (Models.AckType)e.ReferenceId;
                                }
                                catch { }
                                if (ackType == Models.AckType.登录)
                                {
                                    cfg.IServerConfig.IsAgentLogin = true;
                                    cfg.IServerConfig.AgentStatus = Models.AgentStatus.就绪;
                                    CallJavaScript("HandlerIServerEvent", "EventAck_AgentLogin", response.ToString(), errorCode, interactionId);
                                    return;
                                }
                                else if (ackType == Models.AckType.注销)
                                {
                                    cfg.IServerConfig.IsAgentLogout = true;
                                }
                                else if (ackType == Models.AckType.就绪)
                                {
                                    cfg.IServerConfig.AgentStatus = Models.AgentStatus.就绪;
                                    CallJavaScript("HandlerIServerEvent", "EventAck_Ready", response.ToString(), errorCode, interactionId);
                                    return;
                                }
                                else if (e.ReferenceId >= 99 && e.ReferenceId <= 108)
                                {
                                    cfg.IServerConfig.AgentStatus = (Models.AgentStatus)(e.ReferenceId - 100);
                                    CallJavaScript("HandlerIServerEvent", "EventAck_NotReady", response.ToString(), errorCode, interactionId);
                                    return;
                                }
                                else if (ackType == Models.AckType.接受)
                                {
                                    //无法返回Extension，不知道如何查interactionId
                                    //var ext = e.Extension;
                                    //interactionId = ext.GetAsString("interactionId");
                                    //CallJavaScript("HandlerIServerEvent", "EventAccepted", response.ToString(), errorCode, interactionId);
                                    //return;
                                }
                                else if (ackType == Models.AckType.更新随路)
                                {

                                }
                                var dd = e.ProxyClientId;
                            }
                            break;
                        case ISVR.Events.EventCurrentAgentStatus.MessageId:
                            {
                                var r = (ISVR.Events.EventCurrentAgentStatus)response;

                            }
                            break;
                        case ISVR.Events.EventAgentConnectionClosed.MessageId:
                            {
                                //连接关闭
                                cfg.IServerConfig.IsLinkConnected = false;
                            }
                            break;
                        case ISVR.Events.EventInvite.MessageId:
                            {
                                //有新的交互
                                var r = (ISVR.Events.EventInvite)response;
                                interactionId = r.Interaction.InteractionId;
                                var ticketId = r.TicketId;
                                var userData = r.Interaction.InteractionUserData;
                                var e = r.ProxyClientId;

                                var interaction = cf.Interactions.FirstOrDefault(x => x.InteractionId == interactionId);
                                if (interaction != null)
                                {
                                    interaction.InteractionId = interactionId;
                                    interaction.UserData.Clear();
                                    foreach (var key in userData.AllKeys.Distinct())
                                    {
                                        if (userData[key] != null)
                                        {
                                            var strvalue = (userData[key] ?? "").ToString();
                                            if (key.ToLower() == ("UserName".ToLower()))
                                            {
                                                try
                                                {
                                                    var buffer = Convert.FromBase64String(strvalue);
                                                    strvalue = System.Text.Encoding.UTF8.GetString(buffer);
                                                }
                                                catch { }
                                            }
                                            interaction.UserData.Add(key, strvalue);
                                        }
                                    }
                                    interaction.TicketId = ticketId;
                                }
                                else
                                {
                                    var info = new Models.Interaction();
                                    info.InteractionId = interactionId;
                                    foreach (var key in userData.AllKeys.Distinct())
                                    {
                                        var strvalue = (userData[key] ?? "").ToString();
                                        if (key.ToLower() == ("UserName".ToLower()))
                                        {
                                            try
                                            {
                                                var buffer = Convert.FromBase64String(strvalue);
                                                strvalue = System.Text.Encoding.UTF8.GetString(buffer);
                                            }
                                            catch { }
                                        }
                                        info.UserData.Add(key, strvalue);
                                    }
                                    info.TicketId = ticketId;
                                    cf.Interactions.Add(info);
                                }
                            }
                            break;
                        case ISVR.Events.EventAgentInvited.MessageId:
                            {
                                //已经交互
                                var r = (ISVR.Events.EventAgentInvited)response;
                                interactionId = r.Interaction.InteractionId;
                            }
                            break;
                        case ISVR.Events.EventRevoked.MessageId:
                            {
                                //结束
                                var r = (ISVR.Events.EventRevoked)response;
                                interactionId = r.Interaction.InteractionId;
                                var info = cf.Interactions.Where(x => x.InteractionId == interactionId).FirstOrDefault();
                                if (info != null)
                                {
                                    //不清理 因为还可能会转到自己
                                    //cf.Interactions.Remove(info);
                                }
                            }
                            break;
                        case ISVR.Events.EventError.MessageId:
                            {
                                var r = (ISVR.Events.EventError)response;
                                errorCode = r.ErrorCode;
                                errorResponse = response.ToString();
                                errorMessage = r.ErrorDescription;
                                LogException(new Exception("IS-ERR:" + r.ToString()));
                            }
                            break;
                    }
                    CallJavaScript("HandlerIServerEvent", response.Name, errorResponse, errorCode, errorMessage, interactionId);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
        #endregion

        #region ********************************************服务器连接打开关闭********************************************
        private void InitServerApplicationBlocks()
        {
            try
            {
                LogMessage("InitServerApplicationBlocks:开始");

                protocolManagementService = new ProtocolManagementService();
                protocolManagementService.ProtocolOpened += new EventHandler<ProtocolEventArgs>(this.OnProtocolOpened);
                protocolManagementService.ProtocolClosed += new EventHandler<ProtocolEventArgs>(this.OnProtocolClosed);

                if (cfg.TServerConfig.EnableVoice || cfg.IServerConfig.EnableChat)
                {
                    eventBrokerService = new EventBrokerService(protocolManagementService.Receiver);
                }

                if (cfg.TServerConfig.EnableVoice)
                {
                    LogMessage("TServer注册:开始");
                    tserverConfiguration = new TServerConfiguration(TSERVER_IDENTIFIER);
                    try
                    {
                        tserverConfiguration.Uri = new Uri(cfg.TServerConfig.TSERVER_URI);
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                    }
                    tserverConfiguration.ClientName = cfg.TServerConfig.TSERVER_CLIENT_NAME;
                    tserverConfiguration.ClientPassword = cfg.TServerConfig.TSERVER_CLIENT_PASSWORD;

                    if (cfg.TServerConfig.HASupport)
                    {
                        tserverConfiguration.WarmStandbyAttempts = 3;
                        tserverConfiguration.WarmStandbyTimeout = 3000;
                        tserverConfiguration.WarmStandbyUri = new Uri(cfg.TServerConfig.Bakup_TSERVER_URI);
                        tserverConfiguration.FaultTolerance = FaultToleranceMode.HotStandby;
                    }

                    protocolManagementService.Register(tserverConfiguration);
                    eventBrokerService.Register(
                    TServerEventsHandler,
                    new MessageFilter(protocolManagementService[TSERVER_IDENTIFIER].ProtocolId));
                    protocolManagementService[TSERVER_IDENTIFIER].Opened += TSERVER_Opened;
                    protocolManagementService[TSERVER_IDENTIFIER].Closed += TSERVER_Closed;
                    LogMessage("TServer注册:结束");
                }

                if (cfg.IServerConfig.EnableChat)
                {
                    LogMessage("IServer注册:开始");
                    iserverConfiguration = new InteractionServerConfiguration(ISERVER_IDENTIFIER);
                    iserverConfiguration.Uri = new Uri(cfg.IServerConfig.ISERVER_URI);
                    iserverConfiguration.ClientName = cfg.IServerConfig.ISERVER_CLIENT_NAME;
                    iserverConfiguration.ClientType = Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.InteractionClient.AgentApplication;

                    //此处代码不动 2013.12.09
                    if (cfg.IServerConfig.HASupport)
                    {
                        tserverConfiguration.WarmStandbyAttempts = 3;
                        tserverConfiguration.WarmStandbyTimeout = 3000;
                        tserverConfiguration.WarmStandbyUri = new Uri(cfg.IServerConfig.Bakup_ISERVER_URI);
                        tserverConfiguration.FaultTolerance = FaultToleranceMode.HotStandby;
                    }
                    //end

                    if (cfg.IServerConfig.WarmStandby)
                    {
                        iserverConfiguration.WarmStandbyAttempts = 3;
                        iserverConfiguration.WarmStandbyTimeout = 3000;
                        iserverConfiguration.WarmStandbyUri = new Uri(cfg.IServerConfig.Bakup_ISERVER_URI);
                        iserverConfiguration.FaultTolerance = FaultToleranceMode.WarmStandby;
                    }

                    protocolManagementService.Register(iserverConfiguration);
                    eventBrokerService.Register(
                   IServerEventsHandler,
                   new MessageFilter(protocolManagementService[ISERVER_IDENTIFIER].ProtocolId));
                    protocolManagementService[ISERVER_IDENTIFIER].Opened += ISERVER_Opened;
                    protocolManagementService[ISERVER_IDENTIFIER].Closed += ISERVER_Closed;
                    LogMessage("IServer注册:结束");
                }

                if (cfg.TServerConfig.EnableVoice || cfg.IServerConfig.EnableChat)
                {
                    eventBrokerService.Activate();
                }
                LogMessage("InitServerApplicationBlocks:结束");
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void FinalizeServerApplicationBlocks(object state)
        {
            if (eventBrokerService != null)
            {
                if (eventBrokerService.IsActive)
                {
                    eventBrokerService.Deactivate();
                    eventBrokerService.Dispose();

                    if (cfg.TServerConfig.EnableVoice)
                    {
                        IProtocol protocol = protocolManagementService[TSERVER_IDENTIFIER];
                        if (protocol.State == ChannelState.Opened)
                        {
                            try
                            {
                                protocol.Close(new TimeSpan(0, 0, 5));
                                protocolManagementService.Unregister(TSERVER_IDENTIFIER);
                            }
                            catch (Exception ex)
                            {
                                LogException(ex);
                            }
                        }
                    }
                    if (cfg.IServerConfig.EnableChat)
                    {
                        IProtocol protocol = protocolManagementService[ISERVER_IDENTIFIER];
                        if (protocol.State == ChannelState.Opened)
                        {
                            try
                            {
                                protocol.Close(new TimeSpan(0, 0, 5));
                                protocolManagementService.Unregister(ISERVER_IDENTIFIER);
                            }
                            catch (Exception ex)
                            {
                                LogException(ex);
                            }
                        }
                    }
                }
                eventBrokerService = null;
            }
        }

        private void OnProtocolOpened(object sender, ProtocolEventArgs e)
        {
            try
            {
                if (IsPreAppExit)
                {
                    return;
                }
                if (this.InvokeRequired)
                {
                    if (!AppExit)
                    {
                        this.Invoke(new EventHandler<ProtocolEventArgs>(this.OnProtocolOpened), sender, e);
                    }
                }
                else
                {
                    LogMessage("OnProtocolOpened:" + e.Protocol);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void OnProtocolClosed(object sender, ProtocolEventArgs e)
        {
            try
            {
                if (IsPreAppExit)
                {
                    return;
                }
                if (this.InvokeRequired)
                {
                    if (!AppExit)
                    {
                        this.Invoke(new EventHandler<ProtocolEventArgs>(this.OnProtocolClosed), sender, e);
                    }
                }
                else
                {
                    LogMessage("OnProtocolClosed:" + e.Protocol.ProtocolDescription.ProtocolName);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void ISERVER_Opened(object sender, EventArgs e)
        {
            try
            {
                if (IsPreAppExit)
                {
                    return;
                }
                if (this.InvokeRequired)
                {
                    if (!AppExit)
                    {
                        this.Invoke(new EventHandler<EventArgs>(this.ISERVER_Opened), sender, e);
                    }
                }
                else
                {
                    LogMessage("MediaCall_Opened");
                    cfg.IServerConfig.IsLinkConnected = true;
                    var p = protocolManagementService[ISERVER_IDENTIFIER] as Genesyslab.Platform.OpenMedia.Protocols.InteractionServerProtocol;

                    iServerClientID = p.ServerContext.ProxyId;
                    CallJavaScript("HandlerIServerEvent", "MediaCall_Opened", "ISERVER_Opened", 0);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void ISERVER_Closed(object sender, EventArgs e)
        {
            try
            {
                if (IsPreAppExit)
                {
                    return;
                }
                if (this.InvokeRequired)
                {
                    if (!AppExit)
                    {
                        this.Invoke(new EventHandler<EventArgs>(this.ISERVER_Closed), sender, e);
                    }
                }
                else
                {
                    LogMessage("MediaCall_Closed");

                    CallJavaScript("MediaCall_Closed");

                    if (!cfg.IServerConfig.Exit && !cfg.IServerConfig.IsLinkConnected)
                    {
                        CallJavaScript("IServerReTryConnect");
                    }
                    cfg.IServerConfig.IsLinkConnected = false;
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void TSERVER_Opened(object sender, EventArgs e)
        {
            try
            {
                if (IsPreAppExit)
                {
                    return;
                }
                if (this.InvokeRequired)
                {
                    if (!AppExit)
                    {
                        this.Invoke(new EventHandler<EventArgs>(this.TSERVER_Opened), sender, e);
                    }
                }
                else
                {
                    protocolManagementService[TSERVER_IDENTIFIER].Timeout = new TimeSpan(1000 * 1000 * 30);
                    LogMessage("TSERVER_Opened");
                    cfg.TServerConfig.IsLinkConnected = true;
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void TSERVER_Closed(object sender, EventArgs e)
        {
            try
            {
                if (IsPreAppExit)
                {
                    return;
                }
                if (this.InvokeRequired)
                {
                    if (!AppExit)
                    {
                        this.Invoke(new EventHandler<EventArgs>(this.TSERVER_Closed), sender, e);
                    }
                }
                else
                {
                    LogMessage("TSERVER_Closed");
                    if (!cfg.TServerConfig.Exit && !cfg.TServerConfig.IsLinkConnected)
                    {
                        CallJavaScript("TServerReTryConnect");
                    }
                    cfg.TServerConfig.IsLinkConnected = false;
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
        #endregion

        //ts
        private string TSERVER_IDENTIFIER = string.Empty;
        private string DN = string.Empty;

        //is
        private string ISERVER_IDENTIFIER = string.Empty;

        //是否已经配置了
        private bool SetConfiged = false;

        private Models.SoftPhoneConfig cfg = new Models.SoftPhoneConfig();
        private Models.VoiceFlow vf = new Models.VoiceFlow();
        private Models.ChatFlow cf = new Models.ChatFlow();

        /// <summary>
        /// 测试客户端是否能调用控件
        /// </summary>
        /// <returns></returns>
        public bool ClientTest
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 配置config
        /// </summary>
        /// <param name="softPhoneConfigJson">Models.SoftPhoneConfig Json string</param>
        public void SetConfig(string softPhoneConfigJson)
        {
            LogMessage("SetConfig:开始");
            try
            {
                if (!SetConfiged)
                {
                    cfg = Utils.JsonHelper.Deserialize<Models.SoftPhoneConfig>(softPhoneConfigJson);

                    //config.TSERVER_IDENTIFIER = "localhost_SoftPhone_OCX";
                    //config.TSERVER_URI = "tcp://10.99.36.253:3000";
                    //config.ISERVER_CLIENT_NAME = "SoftPhone";
                    //config.TSERVER_AGENT_ID = "7636159";
                    //config.TSERVER_DN = "6037735";
                    //config.TSERVER_PASSWORD = "";
                    //config.TSERVER_QUEUE = "";

                    TSERVER_IDENTIFIER = cfg.TServerConfig.TSERVER_IDENTIFIER;
                    DN = cfg.TServerConfig.DN;
                    if (string.IsNullOrEmpty(DN))
                    {
                        cfg.TServerConfig.EnableVoice = false;
                    }
                    if (string.IsNullOrEmpty(TSERVER_IDENTIFIER))
                    {
                        TSERVER_IDENTIFIER = "Lenovo_Voice_OCX";
                    }

                    ISERVER_IDENTIFIER = cfg.IServerConfig.ISERVER_IDENTIFIER;
                    if (string.IsNullOrEmpty(ISERVER_IDENTIFIER))
                    {
                        ISERVER_IDENTIFIER = "Lenovo_Chat_OCX";
                    }
                    LogMessage("SetConfig:成功");
                }
                else
                {
                    LogMessage("SetConfig:已配置");
                }
                LogMessage("SetConfig:结束");
            }
            catch (Exception e)
            {
                LogException(e);
            }

            if (!SetConfiged)
            {
                SetConfiged = true;
                InitServerApplicationBlocks();

                if (cfg.TServerConfig.EnableVoice)
                {
                    Connect();
                }

                if (cfg.IServerConfig.EnableChat)
                {
                    Connect_ISvr();
                }
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <returns>Models.SoftPhoneConfig Json string</returns>
        public string GetConfig()
        {
            return Utils.JsonHelper.Serializer(cfg);
        }

        public string GetDebugInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("cfg:{0}", Utils.JsonHelper.Serializer(cfg));
            sb.AppendLine("");
            sb.AppendFormat("vf:{0}", Utils.JsonHelper.Serializer(vf));
            sb.AppendLine("");
            sb.AppendFormat("cf:{0}", Utils.JsonHelper.Serializer(cf));
            return sb.ToString();
        }

        /// <summary>
        /// 重置话务流
        /// </summary>
        public void VoiceFlowReset()
        {
            vf = new Models.VoiceFlow();
        }

        /// <summary>
        /// 获取话务流
        /// </summary>
        public string GetVoiceFlow()
        {
            return Utils.JsonHelper.Serializer(vf);
        }

        /// <summary>
        /// 更新话务流
        /// </summary>
        /// <param name="voiceFlowJson"></param>
        public void VoiceFlowUpdate(string voiceFlowJson)
        {
            vf = Utils.JsonHelper.Deserialize<Models.VoiceFlow>(voiceFlowJson);
        }

        /// <summary>
        /// 获取chat流
        /// </summary>
        /// <param name="interactionId"></param>
        /// <returns></returns>
        public string GetChatFlow(string interactionId)
        {
            var q = cf.Interactions.Where(x => x.InteractionId == interactionId).FirstOrDefault();
            return Utils.JsonHelper.Serializer(q);
        }

        /// <summary>
        /// inbound是否通话中，通话中不再接振铃事件
        /// </summary>
        public bool IsTalking = false;
        public void SetTalking(bool IsTalking)
        {
            this.IsTalking = IsTalking;
        }


        /// <summary>
        /// 是否正在外拨中,外拨中不再接振铃事件
        /// </summary>
        public bool IsDialing = false;
        public void SetDialing(bool IsDialing)
        {
            this.IsDialing = IsDialing;
        }

        #region Type Declarations
        private delegate void GenesysEventHandlerDelegate(IMessage message);
        #endregion

        #region ********************************************TServer APIs********************************************
        #region 连接 注册 登录 就绪 未就绪
        public void Connect()
        {
            LogMessage("Connect:开始");
            try
            {
                IProtocol protocol = protocolManagementService[TSERVER_IDENTIFIER];
                if (protocol.State == ChannelState.Closed)
                {
                    if (cfg.TServerConfig.ConnectionTimeOut > 0)
                    {
                        protocol.Timeout = new TimeSpan(1000 * 1000 * cfg.TServerConfig.ConnectionTimeOut);
                    }
                    else
                    {
                        protocol.Timeout = new TimeSpan(1000 * 1000 * 5);
                    }
                    LogMessage("Connect:BeginOpen");
                    protocol.BeginOpen();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            LogMessage("Connect:结束");
        }

        public void Disconnect()
        {
            LogMessage("Disconnect:开始");
            try
            {
                IProtocol protocol = protocolManagementService[TSERVER_IDENTIFIER];

                if (protocol.State == ChannelState.Opened)
                {
                    LogMessage("Disconnect:BeginClose");
                    protocol.BeginClose();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            LogMessage("Disconnect:结束");
        }

        public void RegisterAddress()
        {
            try
            {
                RequestRegisterAddress request =
                    RequestRegisterAddress.Create(
                    DN,                               // DN to register
                    RegisterMode.ModeShare,             // Share DN info with other apps?
                    ControlMode.RegisterDefault,        // Register DN with switch?
                    AddressType.DN);                    // Type of DN

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        public void UnregisterAddress()
        {
            try
            {
                RequestUnregisterAddress request =
                    RequestUnregisterAddress.Create(
                    DN,                               // DN to unregister
                    ControlMode.RegisterDefault);       // Unregister DN with switch?

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        public void AgentLogin()
        {
            try
            {
                RequestAgentLogin request =
                    RequestAgentLogin.Create(
                    DN,
                    AgentWorkMode.Unknown);

                request.ThisQueue = cfg.TServerConfig.TSERVER_QUEUE;
                request.AgentID = cfg.TServerConfig.AgentID;
                request.Password = cfg.TServerConfig.Password;

                LogRequest(request.ToString());
                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        public void AgentLogout()
        {
            try
            {
                RequestAgentLogout request =
                    RequestAgentLogout.Create(DN);

                // Provide additional information for request
                request.ThisQueue = cfg.TServerConfig.TSERVER_QUEUE;            // ACD Queue agent will use

                LogRequest(request.ToString());
                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        public void AgentReady()
        {
            try
            {
                RequestAgentReady request =
                    RequestAgentReady.Create(
                    DN,                               // DN to login
                    AgentWorkMode.ManualIn);             // ACW mode

                // Provide additional information for request
                //request.ThisQueue = QUEUE;            // ACD Queue agent will use

                LogRequest(request.ToString());
                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        public void AgentNotReady(string reasonCode, string reasonName, string lenovoReasonCode, bool? AfterCall)
        {
            try
            {
                RequestAgentNotReady request =
                    RequestAgentNotReady.Create(
                    DN,
                    AgentWorkMode.Unknown,
                    cfg.TServerConfig.TSERVER_QUEUE,
                    null,
                    null);

                //挂断电话时写 AgentWorkMode.AfterCallWork，其他使用 AgentWorkMode.ManualIn

                // Provide additional information for request
                //request.ThisQueue = config.TSERVER_QUEUE;            // ACD Queue agent will use

                if (!string.IsNullOrEmpty(reasonCode))
                {
                    var reasons = new KeyValueCollection();
                    reasons.Add("ReasonCode", lenovoReasonCode);
                    reasons.Add("OCXReasonCode", reasonCode + ":" + reasonName);
                    request.Reasons = reasons;
                    //request.Extensions = reasons;
                }
                if (AfterCall.HasValue && AfterCall.Value)
                {
                    request.AgentWorkMode = AgentWorkMode.AfterCallWork;
                }
                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }
        #endregion

        #region 应答 挂断 外拨 保持 取回
        //应答
        public void AnswerCall(string connID)
        {
            try
            {
                RequestAnswerCall request =
                    RequestAnswerCall.Create(
                    DN,
                   new ConnectionId(connID));

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        //挂断
        public void ReleaseCall(string connID)
        {
            try
            {
                RequestReleaseCall request =
                    RequestReleaseCall.Create(
                    DN,
                    new ConnectionId(connID));

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        //外拨
        public void MakeCall(string otherDN, string userDataListKeyValueJson)
        {
            try
            {
                RequestMakeCall request =
                    RequestMakeCall.Create(
                    DN,
                    otherDN,
                    MakeCallType.Regular);

                KeyValueCollection data = Utils.JsonHelper.Deserialize<KeyValueCollection>(userDataListKeyValueJson);
                request.UserData = data;
                vf.ANI = this.DN;
                if (otherDN.StartsWith("#"))
                {
                    vf.DNIS = otherDN.Substring(1);
                    request.OtherDN = otherDN.Substring(1);
                    vf.CallType = CallType.Internal;
                }
                else
                {
                    request.OtherDN = otherDN;
                    vf.DNIS = otherDN;
                    vf.CallType = CallType.Outbound;
                }
                vf.ThisDN = cfg.TServerConfig.DN;
                vf.InOut = 2;

                foreach (var key in data.AllKeys)
                {
                    var val = (data[key] ?? "").ToString();
                    if (vf.UserData.ContainsKey(key))
                    {
                        vf.UserData[key] = val;
                    }
                    else
                    {
                        vf.UserData.Add(key, val);
                    }
                }

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        //保持
        public void HoldCall(string connID)
        {
            try
            {
                RequestHoldCall request =
                    RequestHoldCall.Create(
                    DN,
                  new ConnectionId(connID));

                LogRequest(request.ToString());
                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        //取回
        public void RetrieveCall(string connID)
        {
            try
            {
                RequestRetrieveCall request =
                    RequestRetrieveCall.Create(
                    DN,
                   new ConnectionId(connID));

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }
        #endregion

        #region MuteOn MuteOff
        public void SetMuteOn(string connID)
        {
            try
            {
                RequestSetMuteOn request =
                    RequestSetMuteOn.Create(
                    DN,
                   new ConnectionId(connID));

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        public void SetMuteOff(string connID)
        {
            try
            {
                RequestSetMuteOff request =
                    RequestSetMuteOff.Create(
                    DN,
                   new ConnectionId(connID));

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }
        #endregion

        #region 转接、会议
        //盲转:转满意度
        public void MuteTransfer(string connID, string otherDN)
        {
            try
            {
                RequestMuteTransfer request =
                    RequestMuteTransfer.Create(
                    DN,
                   new ConnectionId(connID),
                    otherDN);

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        //请求转接
        public void InitiateTransfer(string connID, string otherDN)
        {
            try
            {
                RequestInitiateTransfer request =
                    RequestInitiateTransfer.Create(
                    DN,
                    new ConnectionId(connID),
                    otherDN);

                if (otherDN.StartsWith("#"))
                {
                    request.OtherDN = otherDN.Substring(1);
                }

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        //确认转接
        public void CompleteTransfer(string connID, string transferConnID)
        {
            try
            {
                RequestCompleteTransfer request =
                    RequestCompleteTransfer.Create(
                    DN,
                    new ConnectionId(connID),
                    new ConnectionId(transferConnID));

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        //赞时不用
        public void RouteCall(string thisDN, string connID, string otherDN)
        {
            try
            {
                RequestRouteCall request =
                    RequestRouteCall.Create(
                    thisDN,
                    new ConnectionId(connID),
                    otherDN,
                    RouteType.Unknown);

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        //邀请会议
        //报工号、转电话支付:使用会议模式（邀请会议，确认会议，hold住1线，1线通话结束，取回0线） 
        public void InitiateConference(string connID, string otherDN)
        {
            try
            {
                RequestInitiateConference request =
                    RequestInitiateConference.Create(
                    DN,
                    new ConnectionId(connID),
                    otherDN);
                if (otherDN.StartsWith("#"))
                {
                    request.OtherDN = otherDN.Substring(1);
                }

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        //确认会议
        public void CompleteConference(string connID, string conferenceConnID)
        {
            try
            {
                RequestCompleteConference request =
                    RequestCompleteConference.Create(
                    DN,
                   new ConnectionId(connID),
                   new ConnectionId(conferenceConnID));

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }
        #endregion

        #region 随路数据
        public void UpdateUserData(string connID, string userDataListKeyValueJson)
        {
            try
            {
                RequestUpdateUserData request =
                    RequestUpdateUserData.Create(
                    DN,
                    new ConnectionId(connID),
                    null);

                KeyValueCollection data = Utils.JsonHelper.Deserialize<KeyValueCollection>(userDataListKeyValueJson);
                request.UserData = data;

                //同时也更新到本地 2013.03.21
                foreach (var key in data.AllKeys)
                {
                    var val = (data[key] ?? "").ToString();
                    if (vf.UserData.ContainsKey(key))
                    {
                        vf.UserData[key] = val;
                    }
                    else
                    {
                        vf.UserData.Add(key, val);
                    }
                }

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        public void DeleteUserData(string connID)
        {
            try
            {
                RequestDeleteUserData request = RequestDeleteUserData.Create(
                    DN,
                  new ConnectionId(connID));

                LogRequest(request.ToString());

                protocolManagementService[TSERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 1);
                LogException(e);
            }
        }

        public string GetUserData(string connID)
        {
            return Utils.JsonHelper.Serializer(vf.UserData);
        }
        #endregion
        #endregion

        #region ********************************************InteractionServer APIS********************************************
        #region 连接 反连接
        /// <summary>
        /// 连接
        /// </summary>
        public void Connect_ISvr()
        {
            LogMessage("打开ISvr：begin");
            try
            {
                IProtocol protocol = protocolManagementService[ISERVER_IDENTIFIER];
                if (cfg.IServerConfig.ConnectionTimeOut > 0)
                {
                    protocol.Timeout = new TimeSpan(1000 * 1000 * cfg.IServerConfig.ConnectionTimeOut);
                }
                else
                {
                    protocol.Timeout = new TimeSpan(1000 * 1000 * 5);
                }
                if (protocol.State == ChannelState.Closed)
                {
                    protocol.BeginOpen();
                    LogMessage("ISvr:BeginOpen");
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
            LogMessage("打开ISvr：end");
        }

        /// <summary>
        /// 反连接
        /// </summary>
        public void Disconnect_ISvr()
        {
            LogMessage("关闭ISvr：begin");
            try
            {
                IProtocol protocol = protocolManagementService[ISERVER_IDENTIFIER];
                if (protocol.State == ChannelState.Opened)
                {
                    protocol.BeginClose();
                    LogMessage("ISvr:BeginClose");
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
            LogMessage("关闭ISvr：end");
        }
        #endregion

        #region 登录,移除,退出
        public void AgentLogin_ISvr()
        {
            LogMessage("登录ISvr：begin");
            try
            {
                var tenantId = cfg.IServerConfig.TenantId;
                var placeId = cfg.IServerConfig.PlaceID;
                var agentId = cfg.IServerConfig.AgentID;
                ISVR.Requests.AgentManagement.RequestAgentLogin request = ISVR.Requests.AgentManagement.RequestAgentLogin.Create(tenantId, placeId, null);
                request.AgentId = agentId;
                request.MediaList = new KeyValueCollection(); ;
                request.MediaList.Add("chat", 1);
                request.ReferenceId = (int)Models.AckType.登录;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
            LogMessage("登录ISvr：end");
        }

        public void RemoveMedia_ISvr(string media)
        {
            try
            {
                ISVR.Requests.AgentManagement.RequestRemoveMedia request = ISVR.Requests.AgentManagement.RequestRemoveMedia.Create(media, null);
                request.ReferenceId = (int)Models.AckType.未知;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }

        public void AgentLogout_ISvr()
        {
            LogMessage("注销ISvr：begin");
            try
            {
                ISVR.Requests.AgentManagement.RequestAgentLogout request = ISVR.Requests.AgentManagement.RequestAgentLogout.Create();
                request.ReferenceId = (int)Models.AckType.注销;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
            LogMessage("注销ISvr：end");
        }
        #endregion

        #region 就绪、未就绪、修改未就绪原因
        //就绪
        public void CancelNotReadyForMedia_ISvr()
        {
            try
            {
                ISVR.Requests.AgentManagement.RequestCancelNotReadyForMedia request = ISVR.Requests.AgentManagement.RequestCancelNotReadyForMedia.Create("chat", ISVR.ReasonInfo.Create("0", "就绪"));
                request.ReferenceId = (int)Models.AckType.就绪;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }

        //未就绪
        public void NotReadyForMedia_ISvr(string code, string description, int lenovoReasonCode)
        {
            try
            {
                var request = ISVR.Requests.AgentManagement.RequestNotReadyForMedia.Create(
                    "chat",
                    ISVR.ReasonInfo.Create(code, description, lenovoReasonCode));
                request.ReferenceId = int.Parse(code) + 100;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }

        //修改未就绪原因 2013年3月26日
        public void ChangeMediaStateReason_ISvr(string code, string description, int lenovoReasonCode)
        {
            try
            {
                var request = ISVR.Requests.AgentManagement.RequestChangeMediaStateReason.Create(
                    iServerClientID,
                    null,
                    "chat",
                    ISVR.ReasonInfo.Create(code, description, lenovoReasonCode));
                request.ReferenceId = int.Parse(code) + 100;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }
        #endregion

        #region 接受、拒绝、结束、转接个人、转队列、会议、离开
        //接受
        public void Accept_ISvr(int ticketId, string interactionId)
        {
            try
            {
                var request = ISVR.Requests.InteractionDelivery.RequestAccept.Create(ticketId, interactionId);
                request.ReferenceId = (int)Models.AckType.接受;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }

        //拒绝
        private void Reject_ISvr(int ticketId, string interactionId)
        {
            try
            {
                var request = ISVR.Requests.InteractionDelivery.RequestReject.Create(ticketId,
                        interactionId,
                        null);
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }

        //结束
        public void StopProcessing_ISvr(string interactionId)
        {
            try
            {
                var request = ISVR.Requests.InteractionManagement.RequestStopProcessing.Create(
                    interactionId,
                    null);
                request.ReferenceId = (int)Models.AckType.结束;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }

        //转个人
        public void Transfer_ISvr(string interactionId, string agentId, string placeId)
        {
            try
            {
                var request = ISVR.Requests.InteractionManagement.RequestTransfer.Create(
                    iServerClientID, //int proxyClientId
                    null,//extension
                    interactionId,//interactionId
                    agentId,//queue
                    placeId//hold
                    );
                request.ReferenceId = (int)Models.AckType.转个人;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }

        //转队列
        public void PlaceInQueue_ISvr(string interactionId, string queue)
        {
            try
            {
                var request = ISVR.Requests.InteractionManagement.RequestPlaceInQueue.Create(
                    iServerClientID, //int proxyClientId
                    null,//extension
                    interactionId,//interactionId
                    queue,//queue
                    true,//hold
                    null//reason
                    );
                request.ReferenceId = (int)Models.AckType.转队列;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }

        //会议
        public void Conference_ISvr(string interactionId, string agentId, string placeId)
        {
            try
            {
                var request = ISVR.Requests.InteractionManagement.RequestConference.Create(
                    iServerClientID,
                    null,
                    interactionId,
                    agentId,
                    placeId,
                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.VisibilityMode.Conference
                    );
                request.ReferenceId = (int)Models.AckType.邀请会议;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }

        //离开
        public void LeaveInteraction_ISvr(string interactionId)
        {
            try
            {
                var request = ISVR.Requests.InteractionManagement.RequestLeaveInteraction.Create(interactionId, null);
                request.ReferenceId = (int)Models.AckType.离开;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }
        #endregion

        #region 更新随路 获取随路
        //更新随路数据
        public void ChangeProperties_ISvr(string interactionId, string userDataListKeyValueJson)
        {
            try
            {
                var data = Utils.JsonHelper.Deserialize<KeyValueCollection>(userDataListKeyValueJson);
                var addData = new KeyValueCollection();
                var changedData = new KeyValueCollection();
                var request = ISVR.Requests.InteractionManagement.RequestChangeProperties.Create(interactionId);

                var interaction = cf.Interactions.FirstOrDefault(x => x.InteractionId == interactionId);
                if (interaction != null)
                {
                    //交集
                    var changeKeys = interaction.UserData.Keys.Intersect(data.AllKeys).ToList();
                    foreach (var key in changeKeys)
                    {
                        if (data[key] != null)
                        {
                            interaction.UserData[key] = data[key].ToString();
                            changedData.Add(key, data[key].ToString());
                        }
                    }

                    //差集
                    var addKeys = data.AllKeys.Except(interaction.UserData.Keys).ToList();
                    foreach (var key in addKeys)
                    {
                        if (data[key] != null)
                        {
                            interaction.UserData.Add(key, data[key].ToString());
                            changedData.Add(key, data[key].ToString());
                        }
                    }
                }

                if (addData.Count > 0)
                {
                    request.AddedProperties = addData;
                }

                if (changedData.Count > 0)
                {
                    request.ChangedProperties = changedData;
                }

                request.ReferenceId = (int)Models.AckType.更新随路;
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                OcxError(e, 2);
                LogException(e);
            }
        }

        //获取随路数据
        public string GetUserData_ISvr(string interactionId)
        {
            var userdata = new Dictionary<string, string>();
            var interaction = cf.Interactions.FirstOrDefault(x => x.InteractionId == interactionId);
            if (interaction != null)
            {
                userdata = interaction.UserData;
            }
            return Utils.JsonHelper.Serializer(userdata);
        }
        #endregion
        #endregion

        #region SetPreQuit
        public void SetPreAppExit()
        {
            IsPreAppExit = true;
        }
        #endregion

        /// <summary>
        /// 安全退出
        /// </summary>
        public bool Exit()
        {
            if (!AppExit)
            {
                try
                {
                    AppExit = true;
                    cfg.TServerConfig.Exit = true;
                    cfg.IServerConfig.Exit = true;
                    FinalizeServerApplicationBlocks(null);

                    this.Dispose(true);
                }
                catch (Exception err)
                {
                    LogException(err);
                }
            }
            return true;
        }
    }

#pragma warning restore 612, 618
}
