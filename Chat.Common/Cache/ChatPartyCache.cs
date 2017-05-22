using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Chat.Common;
using Chat.Common.Caching;
using Genesyslab.Platform.Commons.Protocols;

namespace Chat.Common.Caching
{
    /// <summary>
    /// 联想用户
    /// </summary>
    public class ChatPartyCache : ChatCacheBase<ChatParty>
    {
        #region Constructors

        private static ChatPartyCache Instance = null;
        private ChatPartyCache()
        {
        }

        public static ChatPartyCache GetInstance()
        {
            if (ChatPartyCache.Instance == null) ChatPartyCache.Instance = new ChatPartyCache();
            return ChatPartyCache.Instance;
        }
        #endregion


        #region Properties

        public override string CacheKey
        {
            get
            {
                return "ChatParty";
            }
        }

        public ReadOnlyCollection<ChatParty> Partys
        {
            get
            {
                return this[this.CacheKey].AsReadOnly();
            }
        }

        #endregion

        #region Methods

        public ChatParty GetParty(string userID)
        {
            if (string.IsNullOrEmpty(userID)) return null;
            return this.Find(item => item.UserID == userID);
        }

        public void RemoveParty(string userID)
        {
            if (string.IsNullOrEmpty(userID)) return;

            ChatParty party = this.Find(item => item.UserID == userID);
            if (party != null)
            {
                try
                {
                    if (party.ChatProtocol != null)
                    {
                        if (party.ChatProtocol.State == ChannelState.Opened)
                            party.ChatProtocol.Close();
                        party.ChatProtocol.Dispose();
                    }
                    if (party.ThreadInvoker != null)
                        party.ThreadInvoker.Dispose();
                    if (party.EventBroker != null)
                        party.EventBroker.ReleaseReceivers();

                    party.ChatProtocol = null;
                    party.ThreadInvoker = null;
                    party.EventBroker = null;
                    this.RemoveItem(party);
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
            }
        }


        public void LogCurrentPartyInfo()
        {
            try
            {
                int threadCount = 0, brokerCount = 0, protocolCount = 0, aliveCount = 0;
                List<ChatParty> lst = this.Partys.ToList();
                lst.ForEach(item =>
                {
                    if (item.EventBroker != null) brokerCount++;
                    if (item.ChatProtocol != null) protocolCount++;
                    if (item.ChatProtocol != null && item.ChatProtocol.State == ChannelState.Opened) aliveCount++;
                    if (item.ThreadInvoker != null) threadCount++;
                });

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Chat总数({0}) 有效Chat({1}) 无效Chat({2}) Service({3}) EventBroker({4})"
                    , lst.Count, aliveCount, protocolCount - aliveCount, threadCount, brokerCount);

                ChatLog.GetInstance().FormatMessage("统计信息：{0}", sb.ToString());
            }
            catch { }
        }

        #endregion
    }

}
