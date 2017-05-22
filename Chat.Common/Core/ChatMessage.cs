using System;
using System.Collections;
using System.Collections.Generic;

namespace Chat.Common
{
    #region ChatMessage

    /// <summary>
    /// 聊天内容
    /// </summary>
    public class ChatMessage
    {
        #region Constructors

        public ChatMessage()
        {
            this.CreateData = DateTime.Now;
            this.StartAt = DateTime.Now;
        }
        #endregion

        #region Properties

        /// <summary>
        /// 接收者ID
        /// </summary>
        public string ReceiverID { get; set; }

        /// <summary>
        /// 发言者编号
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 发言者昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 信息类别
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// 消息创建时间
        /// </summary>
        public DateTime CreateData { get; set; }

        /// <summary>
        /// 发生时间:chatserver的StartAt+秒TimeShift
        /// </summary>
        public DateTime StartAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TimeShift { get; set; }

        /// <summary>
        /// 用于呈现
        /// </summary>
        public string FormatMessage
        {
            get
            {
                return string.Format("<div class='{2}Box'><div class='{2}'><b>{0} {3:HH:mm:ss}</b></div><div class='MsgContent'> {1} </div></div>"
                    , this.NickName, this.Message
                    , Enum.GetName(typeof(MessageType), this.MessageType)
                    , this.StartAt);
            }
        }
        /// <summary>
        /// 用于呈现
        /// </summary>
        public string FormatMessage_wap
        {
            get
            {
                if (this.MessageType == Common.MessageType.ClientMessage)
                {
                    //return string.Format("<div class='{2}Box'><div class='{2}'><b>{0}</b></div> <div class='Pic'></div> <div class='MsgContent'> {1} <div class='Arrow'></div> </div></div>",
                    //    this.NickName,//0
                    //    this.Message,//1
                    //    Enum.GetName(typeof(MessageType), this.MessageType),//2
                    //    this.StartAt//3
                    //    );

                    return string.Format("<div class='userBox'><div class='con'><img src='~/Content/mobile/images/2.jpg'><P>{0}</P></div><div class='img'><img src='~/Content/mobile/images/user.png' ></div></div>",
                         this.Message);
                }
                else if (this.MessageType == Common.MessageType.Alert || this.MessageType == Common.MessageType.Notice)
                {
                    //return string.Format("<div class='{2}Box'><div class='linebox'><div class='line'></div> <div class='{2}'><b>{0}</b></div> <div class='line line_l'></div><div class='clear'></div></div> <div class='MsgContent'> {1} </div></div>",
                    //    this.NickName,//0
                    //    this.Message,//1
                    //    "AgentMessage",//2
                    //    this.StartAt//3
                    //    );
                    return string.Format("<div class='waiterBox'><div class='img'><img src='~/Content/mobile/images/waiter.png'></div><div class='con'><img src='~/Content/mobile/images/1.jpg'><p>{0}</p></div></div>",
                        this.Message);
                }
                else
                {
                    //return string.Format("<div class='{2}Box'><div class='Pic'></div><div class='{2}'><b>{0}</b></div><div class='MsgContent_t'> {1} <div class='Arrow'></div> </div><div class='clear'></div></div>",
                    //    this.NickName,//0
                    //    this.Message,//1
                    //   "AgentMessage",//Enum.GetName(typeof(MessageType), this.MessageType),//2
                    //    this.StartAt//3
                    //    );
                    return string.Format("<div class='waiterBox'><div class='img'><img src='~/Content/mobile/images/waiter.png'></div><div class='con'><img src='~/Content/mobile/images/1.jpg'><p>{0}</p></div></div>",
                        this.Message);
                }
            }
        }

        #endregion
    }
    #endregion

    #region ChatMessageCollection

    /// <summary>
    /// 聊天信息集合
    /// </summary>
    public class ChatMessageCollection : ICollection<ChatMessage>
    {
        private List<ChatMessage> messages = new List<ChatMessage>();

        #region Add Message

        /// <summary>
        /// 添加聊天消息（普通消息）
        /// </summary>
        public void AddMessage(string userID, string content)
        {
            AddMessage(userID, userID, userID, content, MessageType.Alert);
        }

        /// <summary>
        /// 添加聊天消息
        /// </summary>
        public void AddMessage(string receiverID, string userID, string nickName, string content, MessageType type)
        {
            ChatMessage msg = new ChatMessage();
            msg.ReceiverID = receiverID;
            msg.UserID = userID;
            msg.NickName = nickName;
            msg.MessageType = type;
            msg.Message = content;

            lock (this.messages)
            {
                this.messages.Add(msg);
            }
        }

        #endregion

        #region FindAll

        /// <summary>
        /// 查找第一个匹配的项
        /// </summary>
        public ChatMessage Find(Predicate<ChatMessage> match)
        {
            return this.messages.Find(match);
        }

        /// <summary>
        /// 查找所有匹配的项
        /// </summary>
        public List<ChatMessage> FindAll(Predicate<ChatMessage> match)
        {
            return this.messages.FindAll(match);
        }

        #endregion

        #region ICollection<T>

        /// <summary>
        /// 获取集合包含的元素数。
        /// </summary>
        public int Count { get { return messages.Count; } }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly { get { return true; } }

        /// <summary>
        /// 添加项
        /// </summary>
        public void Add(ChatMessage item)
        {
            lock (this.messages)
            {
                this.messages.Add(item);
            }
        }

        /// <summary>
        /// 清空集合
        /// </summary>
        public void Clear()
        {
            this.messages.Clear();
        }

        /// <summary>
        /// 查找是否包含项
        /// </summary>
        public bool Contains(ChatMessage item)
        {
            return this.messages.Contains(item);
        }

        /// <summary>
        /// 移除项
        /// </summary>
        public bool Remove(ChatMessage item)
        {
            lock (this.messages)
            {
                return this.messages.Remove(item);
            }
        }
        /// <summary>
        /// 把集合从次序位置复制到数组
        /// </summary>
        public void CopyTo(ChatMessage[] array, int arrayIndex)
        {
            this.messages.CopyTo(array, arrayIndex);
        }

        #endregion

        #region IEnumerator<T> IEnumerator

        public IEnumerator<ChatMessage> GetEnumerator()
        {
            return this.messages.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.messages.GetEnumerator();
        }
        #endregion
    }

    #endregion

    #region AgentIndexMessage


    /// <summary>
    /// 和坐席有关的聊天消息
    /// </summary>
    public class AgentIndexMessage
    {
        #region Properties

        /// <summary>
        /// Tab页新增的聊天信息
        /// </summary>
        public List<AgentTabMessage> TabDatas { get; set; }

        #endregion

        #region Constructors

        public AgentIndexMessage()
        {
            this.TabDatas = new List<AgentTabMessage>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(List<LenovoAgent> agents)
        {
            if (agents == null) return;
            agents.ForEach(agent =>
            {
                ChatRoom room = ChatRoomCache.GetInstance().GetRoom(agent.TicketID, agent.InteractionID);
                if (room != null)
                {
                    AgentTabMessage tab = new AgentTabMessage();
                    List<ChatMessage> recentMsgs = room.GetUserMessages();
                    tab.Init(room, agent.UserID, recentMsgs);
                    // 生成ChatID
                    tab.ChatID = string.Format("{0}_{1}", room.SessionID, room.TicketID);
                    this.TabDatas.Add(tab);
                }
            });

        }

        #endregion
    }

    #endregion

    #region AgentTabMessage

    public class AgentTabMessage : ClientChatMsgList
    {
        #region Properties

        /// <summary>
        /// ChatID
        /// </summary>
        public string ChatID { get; set; }

        #endregion

        #region Constructors

        public AgentTabMessage()
            : base()
        {
        }

        #endregion

    }

    #endregion

    #region ClientChatMsgList

    public class ClientChatMsgList
    {
        #region Properties

        /// <summary>
        /// 聊天室状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 对方是否在输入中……
        /// </summary>
        public int IsTyping { get; set; }

        /// <summary>
        /// 消息列表
        /// </summary>
        public List<string> MsgList { get; set; }

        /// <summary>
        /// 最大延迟
        /// </summary>
        public int MaxDelay { get; set; }
        #endregion

        #region Constructors

        public ClientChatMsgList()
        {
            this.MsgList = new List<string>();
            this.Status = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(ChatContextBase context, bool iswap = false)
        {
            if (context == null) return;
            List<ChatMessage> recentMsgs = new List<ChatMessage>();
            if (context.Room != null)
                recentMsgs = context.Room.GetUserMessages();
            Init(context.Room, context.UserID, recentMsgs, iswap);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="room">聊天室</param>
        /// <param name="userID">取消息的用户ID</param>
        /// <param name="recentMsgs">取得的消息</param>
        public void Init(ChatRoom room, string userID, List<ChatMessage> recentMsgs, bool iswap = false)
        {
            int delay = 0, timeShift = 0, isTyping = 0;
            ChatRoomStatus status = ChatRoomStatus.Closed;
            if (room != null)
            {
                TimeSpan ts = DateTime.Now - room.CreateDate;
                if (ts.TotalSeconds > 5000) timeShift = 5000;
                else
                    timeShift = (int)ts.TotalSeconds;

                isTyping = room.IsCustomerTyping ? 1 : 0;
                status = room.Status;
                if (status == ChatRoomStatus.Chatting && !room.IsContainsSelf(userID))
                    status = ChatRoomStatus.Closed;
            }
            recentMsgs.ForEach(item =>
            {
                delay = timeShift - item.TimeShift;
                if (delay > this.MaxDelay) this.MaxDelay = delay;
                if (iswap)
                {
                    this.MsgList.Add(item.FormatMessage_wap);
                }
                else
                {
                    this.MsgList.Add(item.FormatMessage);
                }
            });
            this.IsTyping = isTyping;
            switch (status)
            {
                case ChatRoomStatus.Closed:
                    this.Status = -1;
                    break;
                case ChatRoomStatus.Initting:
                case ChatRoomStatus.WaitService:
                    this.Status = 1;
                    break;
                case ChatRoomStatus.Chatting:
                    this.Status = 2;
                    break;
            }
        }

        #endregion
    }

    #endregion
}
