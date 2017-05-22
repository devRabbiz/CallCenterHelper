using System;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using System.Diagnostics;
using Chat.Common;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Commons.Threading;
using Genesyslab.Platform.WebMedia.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests;

namespace Chat.AgentInterface
{
    public class AgentChatContext : ChatContextBase
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(AgentChatContext));
        #region Constructors

        public AgentChatContext(string ticketID, string sessionID, string agentID)
            : base(ticketID, sessionID)
        {
            this.AgentID = agentID;
            this.SessionID = sessionID;
            this.RoomID = sessionID;
            base.InitRoom();
        }

        public AgentChatContext(string ticketID, string sessionID, LenovoAgent agent)
            : this(ticketID, sessionID, agent.AgentId)
        {
            if (agent == null) throw new Exception("参数 agent 不能为空！");
            this.agent = agent;
        }


        #endregion

        #region Properties

        public string AgentID { get; set; }

        private LenovoAgent agent = null;
        public LenovoAgent Agent
        {
            get
            {
                if (this.agent == null)
                    this.agent = AgentCache.GetInstance().GetAgent(this.TicketID, this.AgentID, this.SessionID);
                return this.agent;
            }
        }

        public override string UserID
        {
            get
            {
                string userID = string.Empty;
                if (this.Agent != null) userID = this.Agent.UserID;
                return userID;
            }
        }

        /// <summary>
        /// 连接是否可用
        /// </summary>
        public override bool IsAvailableConnection
        {
            get
            {
                return this.Agent != null
                   && this.Agent.ChatProtocol != null
                   && this.Agent.ChatProtocol.State == ChannelState.Opened;
            }
        }

        /// <summary>
        /// ChatServer主机名
        /// </summary>
        public string ChatServerHost { get; set; }

        /// <summary>
        /// ChatServer端口
        /// </summary>
        public string ChatServerPort { get; set; }


        #endregion

        #region Methods

        #region 连接

        /// <summary>
        /// 实现基类方法
        /// </summary>
        public override void InitConnecton()
        {
            if (string.IsNullOrEmpty(this.ChatServerHost) || string.IsNullOrEmpty(this.ChatServerPort))
            {
                Trace.WriteLine("异常：获取的访客端ChatServer配置不正确");
                int port = 0;
                int.TryParse(this.ChatServerPort, out port);
                if (port == 0)
                    Trace.WriteLine("ChatServer端口不正确。");
                return;
            }
            Uri csUri = new Uri(string.Format("tcp://{0}:{1}", this.ChatServerHost, this.ChatServerPort));
            this.Agent.ChatProtocol = new BasicChatProtocol(new Endpoint(csUri));
            this.Agent.ChatProtocol.AutoRegister = true;
            this.Agent.ChatProtocol.UserNickname = agent.AgentId;
            this.Agent.ChatProtocol.UserType = UserType.Agent;
            this.Agent.ChatProtocol.Error += ChatProtocol_Error;
            this.Agent.ChatProtocol.Opened += ChatProtocol_Opened;
            this.Agent.ChatProtocol.Closed += ChatProtocol_Closed;

            try
            {
                string logName = "testLog";
                Log4NetLogger logger = new Log4NetLogger(log, logName);

                this.Agent.ThreadInvoker = new SingleThreadInvoker("EventReceivingBrokerService-1");
                this.Agent.ThreadInvoker.EnableLogging(logger);
                this.Agent.EventBroker = new EventReceivingBrokerService(this.Agent.ThreadInvoker);
                this.Agent.EventBroker.EnableLogging(logger);
                this.Agent.EventBroker.Register(this.ChatEventsHandler, new MessageFilter(this.Agent.ChatProtocol.ProtocolId));
                this.Agent.ChatProtocol.SetReceiver(this.Agent.EventBroker);
                this.Agent.ChatProtocol.EnableLogging(logger);

                var isOpend = false;
                lock (ChatLock.LOCK_OPEN)
                {
                    this.Agent.ChatProtocol.Open();
                    isOpend = true;
                    System.Threading.Thread.Sleep(50);
                }
                if (!isOpend) throw new Exception("无法连接消息服务器，请稍候再试。");
            }
            catch (Exception ex)
            {
                if (this.Agent.EventBroker != null)
                    this.Agent.EventBroker.Unregister(this.ChatEventsHandler);
                this.PartyDispose(this.Agent);

                ChatLog.GetInstance().LogException(ex);
            }
        }

        /// <summary>
        /// 加入聊天
        /// </summary>
        /// <param name="interactionID"></param>
        public void ChatJoin(string interactionID, string title)
        {
            ChatLog.GetInstance().FormatMessage("RequestChat请求。连接状态：{2} TicketID:{0} SessionID:{1}"
                , this.TicketID, this.SessionID, this.IsAvailableConnection ? "可用" : "不可用");

            if (string.IsNullOrEmpty(title)) title = "工程师";
            string message = string.Format("{0} [{1}] 为您服务。", title, this.Agent.ChatProtocol.UserNickname);
            if (this.IsAvailableConnection)
                this.ChatJoin(interactionID, this.UserID, message);
        }

        #endregion

        #region 离开

        /// <summary>
        /// 离开聊天室
        /// </summary>
        /// <param name="interactionId"></param>
        public void LeftChat(bool keepAlive)
        {
            try
            {
                if (this.IsAvailableConnection)
                    this.LeftChat(this.UserID, keepAlive);
                this.Room.Partys.Clear();
            }
            catch { }
        }

        #endregion

        #region 聊天/通知

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="interactionId"></param>
        /// <param name="message"></param>
        public void SendMessage(string interactionId, string message)
        {
            if (this.IsAvailableConnection)
                this.SendMessage(interactionId, this.UserID, message);
            else
            {
                message = Microsoft.JScript.GlobalObject.decodeURIComponent(message);
                ChatLog.GetInstance().FormatMessage("异常消息：SessionID:{0},Msg:{1}", interactionId, message);
            }
        }

        protected override void PartyLeftFilter(ChatMessage msg, ChatParty party)
        {
            // 转队列或转人时，接起的坐席端不提示上一坐席离开
            if (party != null) msg.Message = string.Empty;

            base.PartyLeftFilter(msg, party);
        }

        /// <summary>
        /// 过滤消息
        /// </summary>
        /// <param name="party"></param>
        /// <param name="msg"></param>
        protected override void MessageFilter(ChatParty party, ChatMessage msg)
        {
            if (party.UserType == UserType.Agent && msg.Message.IndexOf("$$RTO$$") != -1)
                msg.Message = string.Empty;
            if (party.UserType == UserType.External)
                msg.Message = string.Empty;
            base.MessageFilter(party, msg);
        }

        /// <summary>
        /// 收到消息后对消息的处理
        /// </summary>
        /// <param name="msg"></param>
        protected override void AfterReceivedMessage(string typeName, ChatMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.Message) && typeName != "NewPartyInfoData")
                this.Room.Messages.Add(msg);
        }

        #endregion

        #endregion

        #region Events

        void ChatProtocol_Opened(object sender, EventArgs e)
        {
            // 缓存Agent
            AgentCache.GetInstance().AddItem(this.agent);
            if (!this.Room.Partys.Exists(item => item.UserType == UserType.Agent
                && item.DisplayName == this.agent.DisplayName))
                this.Room.Partys.Add(this.agent);

            // 记录统计信息
            AgentCache.GetInstance().LogCurrentAgentInfo();
            ChatRoomCache.GetInstance().LogCurrentRoomInfo();
        }

        void ChatProtocol_Closed(object sender, EventArgs e)
        {
            ChatLog.GetInstance().FormatMessage("BasicChatProtocol关闭。TicketID:{0},SessionID:{1}"
                , this.TicketID, this.SessionID);

            this.RemoveParty(this.UserID);
        }

        void ChatProtocol_Error(object sender, EventArgs e)
        {
            string errorMessage = "我抓到了BasicChatProtocol异常，怎么显示？";
            //ProtocolException ex = e as ProtocolException;
            if (!string.IsNullOrEmpty(errorMessage))
                ChatLog.GetInstance().FormatMessage(errorMessage);
        }

        #endregion
    }
}
