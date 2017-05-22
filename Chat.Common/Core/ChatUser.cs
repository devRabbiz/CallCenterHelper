using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Genesyslab.Platform.WebMedia.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.Commons.Threading;

namespace Chat.Common
{
    #region ChatUser

    /// <summary>
    /// 聊天用户
    /// </summary>
    public class ChatParty
    {
        #region Properties

        /// <summary>
        /// 用户标识
        /// </summary>
        private string userID = string.Empty;
        public string UserID
        {
            get
            {
                if (this.ChatProtocol != null)
                    userID = this.ChatProtocol.UserId;
                return userID;
            }
            set
            {
                userID = value;
            }
        }

        /// <summary>
        /// 昵称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType { get; protected set; }

        /// <summary>
        /// 控制器
        /// </summary>
        public BasicChatProtocol ChatProtocol { get; set; }

        public EventReceivingBrokerService EventBroker { get; set; }

        public SingleThreadInvoker ThreadInvoker { get; set; }
        #endregion

        #region Constructors

        public ChatParty()
        {
            this.UserType = Genesyslab.Platform.WebMedia.Protocols.BasicChat.UserType.Client;
        }

        public ChatParty(UserType userType)
        {
            this.UserType = userType;
        }
        #endregion
    }

    #endregion

    /// <summary>
    /// 联想客服坐席
    /// </summary>
    public class LenovoAgent : ChatParty
    {
        #region Properties

        /// <summary>
        /// 客服编号
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// 坐席编号
        /// </summary>
        public string PlaceId { get; set; }

        /// <summary>
        /// 票据号
        /// </summary>
        public string TicketID { get; set; }

        /// <summary>
        /// 服务对象排队号
        /// </summary>
        public string InteractionID { get; set; }

        #endregion

        #region Constructors

        public LenovoAgent()
        {
            this.UserType = Genesyslab.Platform.WebMedia.Protocols.BasicChat.UserType.Agent;
        }

        #endregion
    }


}
