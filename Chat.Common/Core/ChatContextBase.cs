using System;
using System.Diagnostics;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.WebMedia.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests;
namespace Chat.Common
{
    /// <summary>
    /// 聊天上下文
    /// </summary>
    public abstract class ChatContextBase
    {
        #region Constructors

        protected ChatContextBase(string ticketID, string sessionID)
        {
            this.TicketID = ticketID;
            this.SessionID = sessionID;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 票据号
        /// </summary>
        public string TicketID { get; set; }

        /// <summary>
        /// 会话标识
        /// </summary>
        public string SessionID { get; protected set; }

        /// <summary>
        /// 当前房间ID
        /// </summary>
        public string RoomID { get; protected set; }

        /// <summary>
        /// 当前用户ID
        /// </summary>
        public abstract string UserID { get; }

        /// <summary>
        /// 连接是否可用
        /// </summary>
        public abstract bool IsAvailableConnection { get; }

        /// <summary>
        /// 聊天室
        /// </summary>
        public ChatRoom Room
        {
            get
            {
                if (string.IsNullOrEmpty(this.SessionID))
                    throw new Exception("参数 SessionID 不合法。");

                ChatRoom room = ChatRoomCache.GetInstance().GetRoom(this.TicketID, this.SessionID);
                return room;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 初始化与ChatServer的连接
        /// 需要在派生类中实现
        /// </summary>
        public abstract void InitConnecton();

        /// <summary>
        /// 离开聊天室
        /// </summary>
        public void LeftChat(string userID, bool keepAlive)
        {
            if (this.RoomID == null && this.Room != null) this.RoomID = this.Room.RoomID;
            RequestReleaseParty leftMessage = null;
            try
            {
                leftMessage = RequestReleaseParty.Create(this.RoomID);
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            if (leftMessage == null) return;
            leftMessage.UserId = userID;
            ChatParty party = this.Room.GetUser(userID);
            if (party != null)
            {
                try
                {
                    if (keepAlive)
                    {
                        leftMessage.MessageText = MessageText.Create(string.Format("[{0}] 已经离开。", party.DisplayName));
                    }
                    else
                    {
                        leftMessage.MessageText = MessageText.Create("亲，网络不给力，您与攻城狮失联了！");
                    }
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
                if (party.UserType == UserType.Agent)
                {
                    leftMessage.AfterAction = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Action.CloseIfNoAgents;
                    if (keepAlive)
                        leftMessage.AfterAction = Genesyslab.Platform.WebMedia.Protocols.BasicChat.Action.KeepAlive;
                }
            }
            this.SendMessage(userID, leftMessage);
        }

        /// <summary>
        /// 加入聊天室
        /// </summary>
        public void ChatJoin(string interactionID, string userID, string message)
        {
            try
            {
                RequestJoin joinMessage = null;
                MessageText msg = MessageText.Create(message);
                if (!string.IsNullOrEmpty(interactionID))
                    joinMessage = RequestJoin.Create(interactionID, Visibility.All, msg);
                else
                    joinMessage = RequestJoin.Create(Visibility.All, "Resources:default", "I have some question.", msg);
                this.SendMessage(userID, joinMessage);
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().FormatMessage("异常：RequestJoin 失败！TicketID：{0} SessionID：{1}", this.TicketID, this.RoomID);
                ChatLog.GetInstance().LogException(ex);
            }
        }

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="message"></param>
        public void SendMessage(string interactionID, string userID, string message)
        {
            if (!string.IsNullOrEmpty(message))
                message = Microsoft.JScript.GlobalObject.decodeURIComponent(message);

            if (string.IsNullOrEmpty(interactionID))
            {
                if (this.Room != null)
                    interactionID = this.Room.RoomID;
                else
                    interactionID = this.Room.SessionID;
            }
            try
            {
                ChatLog.GetInstance().FormatMessage("发送消息：SessionID:{0},Content:{1}", interactionID, message);
                MessageText text = MessageText.Create(message);
                RequestMessage request = RequestMessage.Create(interactionID, Visibility.All, text);
                ChatLog.GetInstance().FormatMessage("发送消息：SessionID:{0}.begin", interactionID);
                this.SendMessage(userID, request);
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            ChatLog.GetInstance().FormatMessage("发送消息：SessionID:{0}.end", interactionID);
        }

        #endregion

        #region Protected Methods

        // 聊天事件
        protected void ChatEventsHandler(IMessage response)
        {
            switch (response.Id)
            {
                case EventSessionInfo.MessageId:
                    {
                        EventSessionInfo info = (EventSessionInfo)response;
                        this.RoomID = info.ChatTranscript.SessionId;
                        if (this.Room != null)
                            this.Room.RoomID = this.RoomID;

                        foreach (var item in info.ChatTranscript.ChatEventList)
                        {
                            try
                            {
                                var StartAt = info.ChatTranscript.StartAt;
                                AddMessage(item, StartAt);
                            }
                            catch (Exception ex)
                            {
                                ChatLog.GetInstance().LogException(ex);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 添加消息到聊天室消息集合
        /// </summary>
        /// <param name="item"></param>
        protected void AddMessage(object item, string StartAt)
        {
            if (item == null) return;
            ChatMessage msg = new ChatMessage();
            msg.ReceiverID = this.UserID;
            msg.MessageType = MessageType.Alert;
            msg.CreateData = DateTime.Now;
            msg.UserID = "system";
            msg.NickName = "系统";

            #region switch message type
            ChatParty party = null;

            string typeName = item.GetType().Name;
            switch (typeName)
            {
                case "MessageInfoData":     // 消息事件
                    MessageInfo chatInfo = (MessageInfo)item;
                    msg.TimeShift = chatInfo.TimeShift;
                    if (chatInfo.MessageText != null)
                        msg.Message = chatInfo.MessageText.Text;
                    msg.UserID = chatInfo.UserId;
                    party = this.Room.Partys.Find(user => user.UserID == msg.UserID);
                    if (party != null)
                    {
                        msg.NickName = party.DisplayName;
                        if (party.UserType == UserType.Agent) msg.MessageType = MessageType.AgentMessage;
                        else if (party.UserType == UserType.Client)
                            msg.MessageType = MessageType.ClientMessage;
                        MessageFilter(party, msg);
                    }
                    break;
                case "NewPartyInfoData":    // 新聊天事件
                    NewPartyInfo joinInfo = (NewPartyInfo)item;
                    msg.TimeShift = joinInfo.TimeShift;
                    if (joinInfo.MessageText != null)
                        msg.Message = joinInfo.MessageText.Text;
                    this.InitRoom();
                    this.AddParty(joinInfo);
                    break;
                //case "NoticeInfoData":      // 通知事件 8.0以前版本不支持通知
                //    NoticeInfo noticeInfo = (NoticeInfo)item;
                //    msg.Message = noticeInfo.NoticeText.Text;
                //    msg.MessageType = MessageType.Notice;

                //    break;
                case "PartyLeftInfoData":   // 离开事件
                    PartyLeftInfo leftInfo = (PartyLeftInfo)item;
                    msg.TimeShift = leftInfo.TimeShift;
                    if (leftInfo.MessageText != null)
                        msg.Message = leftInfo.MessageText.Text;
                    party = this.Room.GetUser(leftInfo.UserId);

                    PartyLeftFilter(msg, party);
                    this.RemoveParty(leftInfo.UserId);
                    break;
            }
            #endregion

            if (!string.IsNullOrEmpty(StartAt))
            {
                msg.StartAt = DateTime.Parse(StartAt).AddSeconds(msg.TimeShift);
            }

            AfterReceivedMessage(typeName, msg);
        }

        protected virtual void PartyLeftFilter(ChatMessage msg, ChatParty party)
        {
        }

        protected virtual void MessageFilter(ChatParty party, ChatMessage msg)
        {
            if (party.UserType == UserType.Agent && msg.Message.IndexOf("$$RTO$$") != -1)
                msg.Message = msg.Message.Replace("$$RTO$$", "");
            if (party.UserType == UserType.Client && msg.Message.IndexOf("$$BROWSER$$") != -1)
            {
                if (msg.Message.IndexOf("$$BROWSER$$??") != -1)
                {
                    msg.Message = string.Empty;
                }
                else
                {
                    msg.Message = msg.Message.Replace("$$BROWSER$$", "");
                    msg.NickName = "系统";
                    msg.MessageType = MessageType.Alert;
                }
            }
            if (party.UserType == UserType.Client && msg.Message.IndexOf("$$CustomerNoticeType$$") != -1)
            {
                string typing = string.Format("$$CustomerNoticeType$$:{0}", NoticeType.TypingStarted);
                this.Room.IsCustomerTyping =
                    msg.Message.Equals(typing, StringComparison.CurrentCultureIgnoreCase);
                msg.Message = string.Empty;
            }
        }


        /// <summary>
        /// 收到消息后的处理
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void AfterReceivedMessage(string typeName, ChatMessage msg)
        {
            if (!string.IsNullOrEmpty(msg.Message))
                this.Room.Messages.Add(msg);
        }

        /// <summary>
        /// 发送消息到ChatServer
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="message"></param>
        protected void SendMessage(string userID, IMessage message)
        {
            ChatParty user = this.Room.Partys.Find(u => u.UserID.Equals(userID, StringComparison.CurrentCultureIgnoreCase));
            if (user != null && user.ChatProtocol != null)
            {
                try
                {
                    ChatLog.GetInstance().FormatMessage("SessionID:{0}, 准备请求Event:{1}", this.SessionID, message.Name);
                    if (user.ChatProtocol.State == ChannelState.Closed)
                    {
                        lock (ChatLock.LOCK_OPEN)
                        {
                            user.ChatProtocol.Open();
                            System.Threading.Thread.Sleep(50);
                        }
                    }
                    if (user.ChatProtocol.State == ChannelState.Opened)
                    {
                        lock (ChatLock.LOCK_REQUEST)
                        {
                            ChatLog.GetInstance().FormatMessage("Event:{0},UserName:{1}.begin", message.Name, user.DisplayName);
                            user.ChatProtocol.Send(message);

                            System.Threading.Thread.Sleep(50);

                            ChatLog.GetInstance().FormatMessage("Event:{0},UserName:{1}.end", message.Name, user.DisplayName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        protected void InitRoom()
        {
            ChatRoom room = ChatRoomCache.GetInstance().GetRoom(this.TicketID, this.SessionID);
            if (room == null)
            {
                room = new ChatRoom(this.TicketID, this.SessionID, this.RoomID);
                ChatRoomCache.GetInstance().AddRoom(room);
            }
            // 日志
            if (!string.IsNullOrEmpty(this.RoomID))
                ChatLog.GetInstance().FormatMessage("SessionID:{0},RoomID:{1}", this.SessionID, this.RoomID);
        }

        /// <summary>
        /// 释放Party资源
        /// </summary>
        /// <param name="party"></param>
        protected void PartyDispose(ChatParty party)
        {
            if (party == null) return;

            if (party.ChatProtocol != null)
            {
                try
                {
                    if (party.ChatProtocol.State == ChannelState.Opened)
                    {
                        // 要不要加锁？
                        party.ChatProtocol.Close();
                    }
                    //party.ChatProtocol.Dispose();
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
            }
            if (party.ThreadInvoker != null)
            {
                try
                {
                    party.ThreadInvoker.Dispose();
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
            }
            if (party.EventBroker != null)
            {
                try
                {
                    party.EventBroker.ReleaseReceivers();
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
            }
            try
            {
                party.ChatProtocol = null;
                party.ThreadInvoker = null;
                party.EventBroker = null;
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
        }

        #endregion

        #region Private Methods

        private void AddParty(NewPartyInfo info)
        {
            if (info == null || info.UserInfo == null) return;

            // 人员
            if (!string.IsNullOrEmpty(info.UserId))
            {
                UserInfo user = info.UserInfo;
                ChatParty party = this.Room.GetUser(info.UserId);
                if (party == null)
                {
                    party = new ChatParty(user.UserType);
                    if (user.UserType == UserType.Agent)
                        party = new LenovoAgent();
                    this.Room.Partys.Add(party);
                }
                party.UserID = info.UserId;
                party.DisplayName = user.UserNickname;

                if (party.UserType == UserType.Agent && this.UserID == party.UserID)
                    this.Room.Messages.AddMessage(this.UserID, "system", "系统", "欢迎您加入聊天室。", MessageType.Alert);

                // 新聊天
                if (party.UserType == UserType.Client)
                    ChatLog.GetInstance().LogEvent(ChatEvent.NewClient, party.DisplayName);
                ChatLog.GetInstance().LogEvent(ChatEvent.PartyJoin, party.DisplayName);
            }
        }

        protected void RemoveParty(string userID)
        {
            ChatRoom room = this.Room;
            if (room == null) return;
            ChatParty party = room.GetUser(userID);
            if (party != null)
            {
                this.PartyDispose(party);
                room.Partys.Remove(party);
                bool hasClient = room.Partys.Exists(item => item.UserType == UserType.Client);
                if (!hasClient && room.Partys.Count > 0)
                    room.Partys.ForEach(item => { this.RemoveParty(item.UserID); });

                ChatLog.GetInstance().LogEvent(ChatEvent.PartyLeftRoom, party.DisplayName);
                if (room.Status == ChatRoomStatus.Closed)
                    ChatLog.GetInstance().LogEvent(ChatEvent.StopChat, party.DisplayName);
            }
        }

        #endregion

    }

}
