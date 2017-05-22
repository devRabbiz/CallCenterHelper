using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Chat.Common;
using Chat.Common.Caching;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Connection;
using Genesyslab.Platform.Commons.Connection.Configuration;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Commons.Threading;
using Genesyslab.Platform.Configuration.Protocols.Types;
using Genesyslab.Platform.WebMedia.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Newtonsoft.Json;

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

        public CustomerChatContext(string sessionID, string queryString)
            : this(sessionID)
        {
            this.customer = InitCustomerInfo(queryString);
        }

        #endregion

        #region Properties
        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool IsInited { get; set; }
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

        public ChatServerInfo ChatServiceInfo { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// 实现基类方法
        /// </summary>
        public override void InitConnecton()
        {
            try
            {
                this.ChatServiceInfo = BalanceCache.GetInstance().GetServiceInfo();
                if (this.ChatServiceInfo == null) return;

                ChatLog.GetInstance().FormatMessage("已分配到服务器。（别名：{0}   地址：{1}:{2}）"
                    , this.ChatServiceInfo.Alias, this.ChatServiceInfo.Host, this.ChatServiceInfo.Port);

                Uri chatRoomURI = new Uri(string.Format("tcp://{0}:{1}", this.ChatServiceInfo.Host, this.ChatServiceInfo.Port));
                Endpoint chatEndPoint = new Endpoint(chatRoomURI);
                ChatParty user = new ChatParty();
                user.DisplayName = this.Customer.DisplayName;
                user.ChatProtocol = new BasicChatProtocol(chatEndPoint);
                user.ChatProtocol.AutoRegister = true;
                user.ChatProtocol.UserType = UserType.Client;
                user.ChatProtocol.UserNickname = this.Customer.DisplayName;
                if (this.Customer.ChatProtocol != null)
                    user.ChatProtocol.UserData = this.Customer.ChatProtocol.UserData;
                user.ChatProtocol.Error += ChatProtocol_Error;
                user.ChatProtocol.Opened += ChatProtocol_Opened;
                user.ChatProtocol.Closed += ChatProtocol_Closed;
                // 打开聊天服务
                try
                {
                    string logName = "testLog";
                    Log4NetLogger logger = new Log4NetLogger(log, logName);
                    user.ThreadInvoker = new SingleThreadInvoker("EventReceivingBrokerService-1");
                    user.ThreadInvoker.EnableLogging(logger);
                    user.EventBroker = new EventReceivingBrokerService(user.ThreadInvoker);
                    user.EventBroker.EnableLogging(logger);
                    user.EventBroker.Register(this.ChatEventsHandler, new MessageFilter(user.ChatProtocol.ProtocolId));
                    user.ChatProtocol.SetReceiver(user.EventBroker);
                    user.ChatProtocol.EnableLogging(logger);
                    this.customer = user;
                    var isOpend = false;
                    lock (ChatLock.LOCK_OPEN)
                    {
                        try
                        {
                            user.ChatProtocol.Open(new TimeSpan(0, 0, 5));
                            isOpend = true;
                        }
                        catch (Exception ex)
                        {
                            ChatLog.GetInstance().LogException(ex);
                        }
                    }
                    if (!isOpend) {
                        ChatLog.GetInstance().FormatMessage("Error03:无法连接消息服务器，方法:CustomerChatContext:InitConnecton，chatRoomURI:{0},用户昵称{1},用户UserID:{2}", chatRoomURI.ToString(), this.Customer.DisplayName, this.Customer.UserID);
                        throw new Exception("无法连接消息服务器，请稍候再试。");
                    }
                       
                }
                catch  
                {
                    if (user.EventBroker != null)
                        user.EventBroker.Unregister(this.ChatEventsHandler);
                    this.PartyDispose(user);
                }
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
        }

        ///// <summary>
        ///// 请求聊天
        ///// </summary>
        //public void RequestChat()
        //{
        //    ChatLog.GetInstance().FormatMessage("RequestChat请求。连接状态：{2} TicketID:{0} SessionID:{1}"
        //        , this.TicketID, this.SessionID, this.IsAvailableConnection ? "可用" : "不可用");
        //    if (this.IsAvailableConnection)
        //        this.ChatJoin(null, this.UserID, string.Empty);
        //}

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMessage(string message)
        {
            if (this.Room == null) return;
            if (this.IsAvailableConnection)
                this.SendMessage(null, this.UserID, message);
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
                    // 防止用户频繁刷新，导致坏死的Chat被接起
                    Thread.Sleep(500);

                    // 先发送一条离开的消息，解决Genesyslab不同版本无法解析退出的BUG
                    this.SendMessage(string.Format("[{0}] 已经离开。", this.Customer.DisplayName));
                    //this.SendMessage("网络异常，服务已中断。");
                    Thread.Sleep(50);
                    this.LeftChat(this.UserID, false);

                    // 从缓存移除
                    ChatPartyCache.GetInstance().RemoveParty(this.UserID);
                }
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

        protected override void MessageFilter(ChatParty party, ChatMessage msg)
        {
            if (party.UserType == UserType.Client && msg.Message.IndexOf("$$BROWSER$$") != -1)
                msg.Message = string.Empty;
            base.MessageFilter(party, msg);
        }

        #endregion

        #region Events


        void ChatProtocol_Opened(object sender, EventArgs e)
        {
            if (!IsInited)
            {
                IsInited = true;
                this.InitRoom();
                this.Room.Partys.Add(this.customer);

                this.ChatJoin(null, this.UserID, string.Empty);

                // 缓存ChatParty
                ChatPartyCache.GetInstance().AddItem(this.customer);

                // 记录统计信息
                ChatPartyCache.GetInstance().LogCurrentPartyInfo();

                // 日志
                ChatLog.GetInstance().LogEvent(ChatEvent.NewClient
                    , string.Format("Name:{0},SessionID:{1}", this.customer.DisplayName, this.SessionID));
            }
        }

        void ChatProtocol_Closed(object sender, EventArgs e)
        {
            ChatLog.GetInstance().FormatMessage("BasicChatProtocol关闭。TicketID:{0},SessionID:{1}"
                , this.TicketID, this.SessionID);
        }

        void ChatProtocol_Error(object sender, EventArgs e)
        {
            string errorMessage = "我抓到了BasicChatProtocol异常，怎么显示？";
            //ProtocolException ex = e as ProtocolException;
            if (!string.IsNullOrEmpty(errorMessage))
                ChatLog.GetInstance().FormatMessage(errorMessage);
        }


        #endregion

        public static ChatParty InitCustomerInfo(string queryString)
        {
            // 初始化用户
            ChatParty customer = new ChatParty();
            if (string.IsNullOrEmpty(queryString)) return customer;
            customer.DisplayName = "联想用户001";
            customer.ChatProtocol = new BasicChatProtocol(new Endpoint("", "", 2020));
            customer.ChatProtocol.UserData = new KeyValueCollection();
            Dictionary<string, object> userData = JsonConvert.DeserializeObject(queryString, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            if (userData != null && userData.Keys.Count > 0)
            {
                string[] keys = new[] { "EnterID", "UserID",  "UserName", "MachineNo" , "TargetSkill", "emailClient"
                    , "RegisterNumber", "Queue", "strServiceCardNo","LAStatID", "WSISID" ,"CustomerIP","CustomerLocation" };
                foreach (string key in keys)
                {
                    if (!userData.Keys.Contains(key)) continue;
                    string value = string.Empty;
                    if (userData[key] != null)
                        value = userData[key].ToString();
                    if (key.Equals("UserName"))
                    {
                        if (string.IsNullOrEmpty(value)) value = "联想用户";
                        customer.DisplayName = value;
                        value = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

                        customer.ChatProtocol.UserData["FirstName"] = value;
                        customer.ChatProtocol.UserData["LastName"] = string.Empty;
                    }
                    customer.ChatProtocol.UserData.Add(key, value);
                }
            }
            return customer;
        }

    }
}
