using System;
using System.Diagnostics;
using System.Threading;
using Chat.Common;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Connection;
using Genesyslab.Platform.Commons.Connection.Configuration;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Commons.Threading;
using Genesyslab.Platform.Configuration.Protocols.Types;
using Genesyslab.Platform.WebMedia.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Genesyslab.WebApi.Core;
using Genesyslab.Platform.Logging;
using Genesyslab.Platform.Logging.Configuration;
using Genesyslab.Platform.Commons.Logging;
using log4net;

namespace Chat.CustomerInterface
{
    public class CustomerChatContext : ChatContextBase
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(CustomerChatContext));
        #region Constructors

        public CustomerChatContext(string sessionID)
            : base(string.Empty, sessionID)
        {
        }

        public CustomerChatContext(string sessionID, ChatParty customerInfo)
            : this(sessionID)
        {
            this.customer = customerInfo;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 当前用户信息
        /// 在初始化之前提供
        /// </summary>
        private ChatParty customer = null;
        public ChatParty Customer
        {
            get
            {
                if (customer == null && this.Room != null)
                    customer = this.Room.Partys.Find(item => item.UserType == UserType.Client);
                return customer;
            }
        }

        public override string UserID
        {
            get
            {
                string userID = string.Empty;
                if (this.Customer != null) userID = this.Customer.UserID;
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
                return this.Customer != null
                    && this.Customer.ChatProtocol != null
                    && this.Customer.ChatProtocol.State == ChannelState.Opened;
                //bool valid = false;
                //if (this.ChatServiceInfo != null)
                //    valid = (this.ChatServiceInfo.IsPrimaryActive | this.ChatServiceInfo.IsBackupActive);
                //return valid;
            }
        }

        /// <summary>
        /// 聊天服务信息
        /// </summary>
        protected ServiceInfo ChatServiceInfo { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// 实现基类方法
        /// </summary>
        public override void InitConnecton()
        {
            int ii = 0;
            while (!this.IsAvailableConnection && ii <= 3)
            {
                Trace.WriteLine(string.Format("正在尝试第{0}次连接……", ii + 1));
                try
                {
                    //this.ChatServiceInfo = LoadBalancer.GetServiceInfo(CfgAppType.CFGChatServer, "Resources");
                    //if (this.ChatServiceInfo == null) continue;

                    //Trace.WriteLine(string.Format("已分配到服务器。（别名：{0}   地址：{1}:{2}）"
                    //    , this.ChatServiceInfo.Alias, this.ChatServiceInfo.Host, this.ChatServiceInfo.Port));

                    //Uri chatRoomURI = new Uri(string.Format("tcp://{0}:{1}", this.ChatServiceInfo.Host, this.ChatServiceInfo.Port));
                    Uri chatRoomURI = new Uri("tcp://10.99.36.115:4801");
                    Endpoint chatEndPoint = new Endpoint(chatRoomURI);
                    ChatParty user = new ChatParty();
                    user.DisplayName = this.Customer.DisplayName;
                    user.ChatProtocol = new BasicChatProtocol(chatEndPoint);
                    user.ChatProtocol.AutoRegister = true;
                    user.ChatProtocol.UserType = UserType.Client;
                    user.ChatProtocol.UserNickname = this.Customer.DisplayName;
                    if (this.Customer.ChatProtocol != null)
                        user.ChatProtocol.UserData = this.Customer.ChatProtocol.UserData;
                    // 打开聊天服务
                    try
                    {
                        string logName = "testLog";
                        Log4NetLogger logger = new Log4NetLogger(log, logName);
                        user.ThreadInvoker = new SingleThreadInvoker("EventReceivingBrokerService-1", logger);
                        user.ThreadInvoker.EnableLogging(logger);
                        user.EventBroker = new EventReceivingBrokerService(user.ThreadInvoker);
                        user.EventBroker.EnableLogging(logger);
                        user.EventBroker.Register(this.ChatEventsHandler, new MessageFilter(user.ChatProtocol.ProtocolId));
                        user.ChatProtocol.SetReceiver(user.EventBroker);

                        var isOpend = false;
                        lock (ChatLock.LOCK_OPEN)
                        {
                            try
                            {
                                user.ChatProtocol.Open();
                                isOpend = true;
                                System.Threading.Thread.Sleep(50);
                            }
                            catch { }
                        }
                        if (!isOpend) throw new Exception("连接ChatServer异常，稍候重试。");

                        this.customer = user;

                        this.InitRoom();
                        this.Room.Partys.Add(this.customer);

                        // 日志
                        ChatLog.GetInstance().LogEvent(ChatEvent.NewClient, this.customer.DisplayName);

                        // 若使用TLS协议
                        if (this.ChatServiceInfo != null
                            && this.ChatServiceInfo.IsWebApiPortSecured)
                        {
                            KeyValueCollection kvs = new KeyValueCollection();
                            kvs[CommonConnection.TlsKey] = 1;
                            kvs[CommonConnection.BlockingModeKey] = "true";
                            KeyValueConfiguration cfg = new KeyValueConfiguration(kvs);
                            this.customer.ChatProtocol.Configure(cfg);
                        }
                    }
                    catch (Exception)
                    {
                        if (user.EventBroker != null)
                            user.EventBroker.Unregister(this.ChatEventsHandler);
                        this.PartyDispose(user);
                    }

                }
                catch (LoadBalancerException ex)
                {
                    Trace.WriteLine("负载异常：" + ex.Message);
                    ii++;
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                    ii++;
                }
            }
        }

        /// <summary>
        /// 请求聊天
        /// </summary>
        public void RequestChat()
        {
            ChatLog.GetInstance().FormatMessage("RequestChat请求。连接状态：{2} TicketID:{0} SessionID:{1}"
                , this.TicketID, this.SessionID, this.IsAvailableConnection ? "可用" : "不可用");
            if (this.IsAvailableConnection)
                this.ChatJoin(null, this.UserID, string.Empty);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMessage(string message)
        {
            if (this.Room == null) return;
            if (this.IsAvailableConnection)
                this.SendMessage(null, this.Customer.UserID, message);
            else
            {
                this.Room.Messages.AddMessage(this.UserID
                    , "system", "系统", "对不起，您已经与系统断开连接，请关闭重连。", MessageType.Alert);
                message = Microsoft.JScript.GlobalObject.decodeURIComponent(message);
                ChatLog.GetInstance().FormatMessage(
                    "异常消息：发送消息时连接不可用。UserID:{0},Customer存在:{1},Msg:{2}"
                    , this.UserID, (this.Customer != null && this.Customer.ChatProtocol != null) ? "Y" : "N", message);
            }
        }

        /// <summary>
        /// 离开聊天室
        /// </summary>
        public void LeftChat()
        {
            try
            {
                if (this.IsAvailableConnection)
                {
                    // 先发送一条离开的消息，解决Genesyslab不同版本无法解析退出的BUG
                    this.SendMessage(string.Format("[{0}] 已经离开。", this.Customer.DisplayName));
                    Thread.Sleep(100);
                    this.LeftChat(this.Customer.UserID, false);
                }
                Thread.Sleep(20);

                this.PartyDispose(this.Customer);
            }
            catch { }
        }

        /// <summary>
        /// 发送输入状态
        /// Paused/Typing
        /// </summary>
        /// <param name="status"></param>
        public void SendTypingNotice(int isStarted)
        {
            try
            {
                NoticeType noticeType = NoticeType.TypingStopped;
                if (isStarted > 0) noticeType = NoticeType.TypingStarted;
                if (this.Room == null) return;

                this.SendMessage(string.Format("$$CustomerNoticeType$$:{0}", noticeType));
            }
            catch { }
        }

        #endregion

    }
}
