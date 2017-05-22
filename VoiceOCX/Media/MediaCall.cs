/*
 * author:zhangsl
 * 2013.03.07
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#region
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;
using Genesyslab.Platform.Commons.Collections;
#endregion
namespace VoiceOCX.Media
{
    public class MediaCall
    {
        private ProtocolManagementService protocolManagementService;
        private InteractionServerConfiguration isConfiguration;
        private EventBrokerService eventBrokerService;

        private string ISERVER_IDENTIFIER = "localhost_SoftPhone_OCX_Chat";
        private string ISERVER_URI = "tcp://10.99.36.253:4420";
        private string CLIENT_NAME = "SoftPhone_OMChatClient";

        private int tenantId = 101;
        private string agentId = "10000328";
        private string placeId = "SOHO0002";

        public delegate void LogEvent(string message);
        public event LogEvent OnLog;

        public Dictionary<string, Models.Interaction> dicInteraction = new Dictionary<string, Models.Interaction>();

        private int ticketId = 0;
        private string interactionId = string.Empty;

        private bool ProtocolOpened = false;
        private bool InteractionServerOpend = false;

        public MediaCall()
        {

        }

        #region 初始化和释放
        public void InitializePSDKApplicationBlocks()
        {
            protocolManagementService = new ProtocolManagementService();

            isConfiguration = new InteractionServerConfiguration(ISERVER_IDENTIFIER);

            try
            {
                isConfiguration.Uri = new Uri(ISERVER_URI);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            isConfiguration.ClientName = CLIENT_NAME;
            isConfiguration.ClientType = InteractionClient.AgentApplication;

            protocolManagementService.Register(isConfiguration);

            protocolManagementService.ProtocolOpened += new EventHandler<ProtocolEventArgs>(this.OnProtocolOpened);
            protocolManagementService.ProtocolClosed += new EventHandler<ProtocolEventArgs>(this.OnProtocolClosed);

            eventBrokerService = new EventBrokerService(protocolManagementService.Receiver);
            eventBrokerService.Register(
                IServerEventsHandler,
                new MessageFilter(protocolManagementService[ISERVER_IDENTIFIER].ProtocolId));

            // Activate event broker service 
            eventBrokerService.Activate();
            protocolManagementService[ISERVER_IDENTIFIER].Opened += MediaCall_Opened;
            protocolManagementService[ISERVER_IDENTIFIER].Closed += MediaCall_Closed;
        }

        void MediaCall_Opened(object sender, EventArgs e)
        {
            InteractionServerOpend = true;
        }

        void MediaCall_Closed(object sender, EventArgs e)
        {
            InteractionServerOpend = false;
        }

        public void FinalizePSDKApplicationBlocks()
        {
            // Cleanup code
            eventBrokerService.Deactivate();

            eventBrokerService.Dispose();

            // Close Connection if opened (check status of protocol object)
            IProtocol protocol = protocolManagementService[ISERVER_IDENTIFIER];

            if (protocol.State == ChannelState.Opened) // Close only if the protocol state is opened
            {
                try
                {
                    protocol.Close();
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
            }

            protocolManagementService.Unregister(ISERVER_IDENTIFIER);
        }
        #endregion

        private void IServerEventsHandler(IMessage response)
        {
            LogResponse(response.ToString());
            switch (response.Id)
            {

                case EventAck.MessageId:
                    {
                        //有响应
                    }
                    break;
                case EventActivityReport.MessageId:
                    {

                    }
                    break;
                case EventAgentAvailable.MessageId:
                    {

                    }
                    break;
                case EventAgentConnectionClosed.MessageId:
                    {
                        //连接关闭
                    }
                    break;
                case EventAgentLogin.MessageId:
                    {
                        //登录成功
                    }
                    break;
                case EventAgentLogout.MessageId:
                    {
                        //退出成功
                    }
                    break;
                case EventAgentNotAvailable.MessageId:
                    {

                    }
                    break;
                case EventAgentStateReasonChanged.MessageId:
                    {

                    }
                    break;
                case EventCurrentAgentStatus.MessageId:
                    {

                    }
                    break;
                case EventInvite.MessageId:
                    {
                        //有新的交互
                        var r = response as EventInvite;
                        var interactionId = r.Interaction.InteractionId;
                        var tickedid = r.TicketId;
                        var userdata = r.Interaction.InteractionUserData;
                        if (!dicInteraction.ContainsKey(interactionId))
                        {
                            dicInteraction.Add(interactionId, new Models.Interaction()
                            {
                                InteractionId = interactionId,
                                State = Models.InteractionState.Alerting,
                                TicketId = tickedid,
                                UserData = userdata
                            });
                        }
                    }
                    break;
                case EventAgentInvited.MessageId:
                    {
                        //已经交互
                        //清理内存 dicInteraction
                        var r = response as EventAgentInvited;
                        var interactionId = r.Interaction.InteractionId;
                        if (dicInteraction.ContainsKey(interactionId))
                        {
                            //dicInteraction.Remove(interactionId);
                        }
                    }
                    break;
                case EventAccepted.MessageId:
                    {
                        //已接受
                    }
                    break;
            }
        }

        private void OnProtocolOpened(object sender, ProtocolEventArgs e)
        {
            ProtocolOpened = true;
        }

        private void OnProtocolClosed(object sender, ProtocolEventArgs e)
        {
            ProtocolOpened = false;
        }

        #region 连接、断开连接
        public void Connect()
        {
            try
            {
                IProtocol protocol = protocolManagementService[ISERVER_IDENTIFIER];
                if (protocol.State == ChannelState.Closed)
                {
                    protocol.BeginOpen();
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void Disconnect()
        {
            try
            {
                IProtocol protocol = protocolManagementService[ISERVER_IDENTIFIER];
                if (protocol.State == ChannelState.Opened)
                {
                    protocol.BeginClose();
                }
                LogMessage("断开成功");
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        #endregion

        #region 登录,退出
        public void AgentLogin()
        {
            try
            {
                RequestAgentLogin request = RequestAgentLogin.Create(tenantId, placeId, null);
                request.AgentId = agentId;
                request.MediaList = new KeyValueCollection(); ;
                request.MediaList.Add("chat", 1);

                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        public void AgentLogout()
        {
            try
            {
                RequestAgentLogout request = RequestAgentLogout.Create();

                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        #endregion

        #region 调试信息记录
        private void LogMessage(string message)
        {
            OnLog("LogMessage:" + message);
        }
        private void LogRequest(string message)
        {
            OnLog("LogRequest:" + message);
        }
        private void LogException(Exception e)
        {
            OnLog("LogException:" + e.ToString());
        }
        private void LogResponse(string message)
        {
            OnLog("LogResponse:" + message);
        }
        #endregion

        #region 就绪未就绪
        //就绪
        public void CancelNotReadyForMedia()
        {
            try
            {
                RequestCancelNotReadyForMedia request = RequestCancelNotReadyForMedia.Create("chat", null);

                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        //未就绪
        public void NotReadyForMedia()
        {
            try
            {

                RequestNotReadyForMedia request = RequestNotReadyForMedia.Create("chat", ReasonInfo.Create("", ""));

                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        #endregion

        #region 接收、拒绝、结束、转接和转接会议
        //接收
        public void Accept(int ticketId, string interactionId)
        {
            try
            {
                RequestAccept request = RequestAccept.Create(ticketId, interactionId);

                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        //拒绝
        private void Reject(int ticketId, string interactionId)
        {
            try
            {
                RequestReject request =
                        RequestReject.Create(ticketId,
                        interactionId,
                        null);
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        //结束
        private void StopProcessing(string interactionId)
        {
            try
            {
                RequestStopProcessing request =
                        RequestStopProcessing.Create(
                        interactionId,
                        null);
                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        //转接、转接会议
        public void Intrude()
        {
            try
            {
                RequestIntrude request = RequestIntrude.Create(ticketId, //int proxyClientId
                    null, //KeyValueCollection extension
                    VisibilityMode.Conference,
                    interactionId);

                LogRequest(request.ToString());
                protocolManagementService[ISERVER_IDENTIFIER].Send(request);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }
        #endregion
    }
}
