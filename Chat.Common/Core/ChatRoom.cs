using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;

namespace Chat.Common
{

    /// <summary>
    /// 聊天室
    /// </summary>
    public class ChatRoom : IDisposable
    {
        #region Constructors

        private ChatRoom()
        {
            this.Partys = new List<ChatParty>();
            this.Messages = new ChatMessageCollection();
            this.CreateDate = DateTime.Now;
        }

        public ChatRoom(string ticketID, string sessionID, string roomID)
            : this()
        {
            this.TicketID = ticketID;
            this.SessionID = sessionID;
            this.RoomID = roomID;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 票据号
        /// </summary>
        public string TicketID { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// 房间编号
        /// </summary>
        public string RoomID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; protected set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                DateTime data = DateTime.Now;
                if (this.Status == ChatRoomStatus.Closed)
                {
                    ChatMessage msg = this.Messages.LastOrDefault();
                    if (msg != null) data = msg.CreateData;
                    else
                        data = this.CreateDate;
                }
                return data;
            }
        }

        /// <summary>
        /// 上次获取消息的时间
        /// </summary>
        public DateTime LastRequest { get; set; }

        /// <summary>
        /// 聊天室状态
        /// </summary>
        public ChatRoomStatus Status
        {
            get
            {
                ChatRoomStatus status = ChatRoomStatus.Initting;
                if (this.Partys.Count == 0) status = ChatRoomStatus.Closed;
                else
                {
                    ChatParty party = this.Partys.Find(item => item.UserType == UserType.Client);
                    if (party != null)
                    {
                        status = ChatRoomStatus.WaitService;
                        party = this.Partys.Find(item => item.UserType == UserType.Agent);
                        if (party != null) status = ChatRoomStatus.Chatting;
                    }
                    else if (this.CreateDate < DateTime.Now.AddSeconds(-10))
                        status = ChatRoomStatus.Closed;
                }
                return status;
            }
        }

        /// <summary>
        /// 聊天内容列表
        /// </summary>
        public ChatMessageCollection Messages { get; protected set; }

        /// <summary>
        /// 联想坐席列表
        /// </summary>
        public List<ChatParty> Partys { get; set; }

        /// <summary>
        /// 访客是否在录入中
        /// </summary>
        public bool IsCustomerTyping { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取用户
        /// </summary>
        internal ChatParty GetUser(string userID)
        {
            ChatParty user = this.Partys.Find(u => u.UserID.Equals(userID, StringComparison.CurrentCultureIgnoreCase));
            return user;
        }

        /// <summary>
        /// 聊天室是否包含自己
        /// </summary>
        public bool IsContainsSelf(string userID)
        {
            bool inRoom = false;
            try
            {
                inRoom = this.Partys.Exists(u => u.UserID.Equals(userID, StringComparison.CurrentCultureIgnoreCase));
            }
            catch { }
            return inRoom;
        }

        /// <summary>
        /// 获取某用户最近的消息
        /// </summary>
        public List<ChatMessage> GetUserMessages()
        {
            List<ChatMessage> result = new List<ChatMessage>();
            try
            {
                ChatLog.GetInstance().FormatDebugMessage("获取消息：准备获取。[TicketID:{0} SessionID:{1}]", this.TicketID, this.RoomID);
                if (this.Messages != null)
                {
                    lock (this.Messages)
                    {
                        DateTime lastRequest = this.LastRequest;
                        this.LastRequest = DateTime.Now;

                        result = this.Messages.FindAll(msg =>
                            msg.CreateData >= lastRequest && msg.CreateData < this.LastRequest);

                        if (result.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("获取消息：区间：[{0:HH:mm:ss.fff}-{1:HH:mm:ss.fff})，共收到{2}条消息。", lastRequest, this.LastRequest, result.Count);
                            for (int ii = 0; ii <= result.Count - 1; ii++)
                                sb.AppendFormat("\r\n\t收到消息：{0}。[TicketID:{1} SessionID:{2}]", result[ii].Message, this.TicketID, this.RoomID);
                            ChatLog.GetInstance().FormatDebugMessage(sb.ToString());
                        }// end if
                    }// end lock
                }
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().FormatMessage("异常信息：获取消息时出错:{0}。[TicketID:{1} SessionID:{2}]"
                    , ex.Message, this.TicketID, this.RoomID);
            }
            return result;
        }


        #endregion

        #region IDisposable

        public void Dispose()
        {
            // 释放 Messages
            if (this.Messages != null)
                this.Messages.Clear();

            // 释放 Party
            if (this.Partys != null)
            {
                this.Partys.ForEach(party =>
                {
                    if (party.ChatProtocol != null)
                    {
                        try
                        {
                            if (party.ChatProtocol.State == ChannelState.Opened)
                                party.ChatProtocol.Close();
                        }
                        catch { }
                        //party.ChatProtocol.Dispose();
                    }
                });
                this.Partys.Clear();
            }
        }

        #endregion
    }

}
