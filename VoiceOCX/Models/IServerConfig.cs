using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VoiceOCX.Models
{
    [Serializable]
    [DataContract]
    public class IServerConfig
    {
        #region Interaction
        /// <summary>
        /// 是否有chat能力.南京没有chat能力
        /// </summary>
        [DataMember]
        public bool EnableChat { get; set; }

        /// <summary>
        /// 连接超时时间 秒
        /// </summary>
        [DataMember]
        public int ConnectionTimeOut { get; set; }

        /// <summary>
        /// 排队服务器标识
        /// </summary>
        [DataMember]
        public string ISERVER_IDENTIFIER { get; set; }

        /// <summary>
        /// 排队服务器连接 tcp://192.168.0.1:4420
        /// </summary>
        [DataMember]
        public string ISERVER_URI { get; set; }

        /// <summary>
        /// 排队服务器的客户端标识
        /// </summary>
        [DataMember]
        public string ISERVER_CLIENT_NAME { get; set; }

        /// <summary>
        /// ?承租方
        /// </summary>
        [DataMember]
        public int TenantId { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        [DataMember]
        public string AgentID { get; set; }

        /// <summary>
        /// 坐席ID
        /// </summary>
        [DataMember]
        public string PlaceID { get; set; }

        /// <summary>
        /// 坐席chat状态
        /// </summary>
        [DataMember]
        public AgentStatus AgentStatus { get; set; }
        #endregion

        #region
        [DataMember]
        public bool IsLinkConnected { get; set; }

        [DataMember]
        public bool IsAgentLogin { get; set; }

        [DataMember]
        public bool IsAgentLogout { get; set; }

        [DataMember]
        public bool Exit { get; set; }
        #endregion

        #region 热备
        /// <summary>
        /// 是否支持热备
        /// </summary>
        [DataMember]
        public bool HASupport { get; set; }

        /// <summary>
        /// 热备链接
        /// </summary>
        [DataMember]
        public string Bakup_ISERVER_URI { get; set; }

        /// <summary>
        /// 只支持WarmStandby，不支持HA
        /// </summary>
        [DataMember]
        public bool WarmStandby { get; set; }
        #endregion

    }
}
