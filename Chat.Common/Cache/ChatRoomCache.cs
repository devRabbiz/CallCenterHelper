using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Chat.Common.Caching;
using System.Linq;
using System.Text;

namespace Chat.Common
{
    /// <summary>
    /// 聊天室缓存
    /// </summary>
    public class ChatRoomCache : ChatCacheBase<ChatRoom>
    {
        #region Constructors

        private static ChatRoomCache Instance = null;
        private ChatRoomCache()
        {
        }

        public static ChatRoomCache GetInstance()
        {
            if (ChatRoomCache.Instance == null) ChatRoomCache.Instance = new ChatRoomCache();
            return ChatRoomCache.Instance;
        }
        #endregion

        #region Properties

        public override string CacheKey
        {
            get
            {
                return "ChatRoom";
            }
        }

        public ReadOnlyCollection<ChatRoom> Rooms
        {
            get
            {
                return this[this.CacheKey].AsReadOnly();
            }
        }

        #endregion

        #region Methods

        public ChatRoom GetRoom(string ticketID, string sessionID)
        {
            return this.Find(item => item.TicketID == ticketID && item.SessionID == sessionID);
        }

        public void AddRoom(ChatRoom room)
        {
            this.AddItem(room);
        }

        public void RemoveRoom(ChatRoom room)
        {
            if (room != null)
            {
                room.Dispose();
                this.RemoveItem(room);
            }
        }

        public void RemoveDeadRooms()
        {
            List<ChatRoom> rms = null;
            // 清理周期配置
            string clearPeriod = System.Configuration.ConfigurationManager.AppSettings["clearPeriod"];
            int seconds = 0;
            int.TryParse(clearPeriod, out seconds);
            if (seconds == 0) seconds = 30;
            if (seconds > 0) seconds *= -1;
            rms = this.FindAll(item => item.CreateDate < DateTime.Now.AddHours(-10) ||
                (item.CreateDate < DateTime.Now.AddSeconds(seconds) && item.LastRequest < DateTime.Now.AddSeconds(seconds)));
            if (rms != null && rms.Count > 0)
            {
                ChatLog.GetInstance().LogEvent(ChatEvent.RemoveCache, "发现无效的Room缓存。");
                rms.ForEach(r =>
                {
                    this.RemoveRoom(r);
                    ChatLog.GetInstance().FormatMessage("\t>>SessionID:{0},RoomID:{1},CreateDate:{2: HH:mm:ss}", r.SessionID, r.RoomID, r.CreateDate);
                });
                ChatLog.GetInstance().FormatMessage("无效的Room缓存移除完毕。", DateTime.Now);
            }
        }

        public void LogCurrentRoomInfo()
        {
            try
            {
                int chattingCount = 0, meetingCount = 0, closedCount = 0;
                List<ChatRoom> lst = this.Rooms.ToList();
                lst.ForEach(item =>
                {
                    if (item.Partys != null)
                    {
                        if (item.Partys.Count >= 2) chattingCount++;
                        if (item.Partys.Count >= 3) meetingCount++;
                    }
                    if (item.Status == ChatRoomStatus.Closed) closedCount++;
                });

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Room总数({0} 普通Chat({1}) 多方支持({2}) ) 已关闭Room({3}) "
                    , lst.Count, chattingCount, meetingCount, closedCount);

                ChatLog.GetInstance().FormatMessage("统计信息：{0}", sb.ToString());
            }
            catch { }
        }
        #endregion
    }
}
