using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Chat.Common;
using Chat.Common.Caching;
using Genesyslab.Platform.Commons.Protocols;

namespace Chat.AgentInterface
{
    /// <summary>
    /// 联想客服
    /// </summary>
    public class AgentCache : ChatCacheBase<LenovoAgent>
    {
        #region Constructors

        private static AgentCache Instance = null;
        private AgentCache()
        {
        }

        public static AgentCache GetInstance()
        {
            if (AgentCache.Instance == null) AgentCache.Instance = new AgentCache();
            return AgentCache.Instance;
        }
        #endregion


        #region Properties

        public override string CacheKey
        {
            get
            {
                return "LenovoAgent";
            }
        }

        public ReadOnlyCollection<LenovoAgent> Agents
        {
            get
            {
                return this[this.CacheKey].AsReadOnly();
            }
        }

        #endregion

        #region Methods

        public LenovoAgent GetAgent(string ticketID, string agentID, string interactionID)
        {
            if (string.IsNullOrEmpty(ticketID)
                || string.IsNullOrEmpty(agentID)
                || string.IsNullOrEmpty(interactionID)) return null;
            return this.Find(item => item.TicketID == ticketID
                && item.AgentId == agentID
                && item.InteractionID == interactionID);
        }

        public List<LenovoAgent> GetAgents(string agentID)
        {
            if (string.IsNullOrEmpty(agentID)) return null;
            return this.FindAll(item => item.AgentId == agentID);
        }

        public void RemoveAgent(string ticketID, string agentID, string interactionID)
        {
            if (string.IsNullOrEmpty(agentID) || string.IsNullOrEmpty(interactionID)) return;

            LenovoAgent agent = this.Find(item => item.TicketID == ticketID && item.AgentId == agentID && item.InteractionID == interactionID);
            if (agent != null)
            {
                try
                {
                    if (agent.ChatProtocol != null)
                    {
                        if (agent.ChatProtocol.State == ChannelState.Opened)
                            agent.ChatProtocol.Close();
                        agent.ChatProtocol.Dispose();
                    }
                    if (agent.ThreadInvoker != null)
                        agent.ThreadInvoker.Dispose();
                    if (agent.EventBroker != null)
                        agent.EventBroker.ReleaseReceivers();

                    agent.ChatProtocol = null;
                    agent.ThreadInvoker = null;
                    agent.EventBroker = null;
                    this.RemoveItem(agent);
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
            }
        }


        public void LogCurrentAgentInfo()
        {
            try
            {
                int threadCount = 0, brokerCount = 0, protocolCount = 0, aliveCount = 0;
                List<LenovoAgent> lst = this.Agents.ToList();
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
