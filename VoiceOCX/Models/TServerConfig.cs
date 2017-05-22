using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VoiceOCX.Models
{
    [Serializable]
    [DataContract]
    public class TServerConfig
    {
        #region Voice
        /// <summary>
        /// 是否有语音能力
        /// </summary>
        [DataMember]
        public bool EnableVoice { get; set; }

        /// <summary>
        /// TSERVER唯一标识
        /// </summary>
        [DataMember]
        public string TSERVER_IDENTIFIER { get; set; }

        /// <summary>
        /// TSERVER服务器连接地址 tcp://192.168.0.1:3000
        /// </summary>
        [DataMember]
        public string TSERVER_URI { get; set; }

        /// <summary>
        /// TSERVER 客户端标识
        /// </summary>
        [DataMember]
        public string TSERVER_CLIENT_NAME { get; set; }

        /// <summary>
        /// 连接密码 默认为空
        /// </summary>
        [DataMember]
        public string TSERVER_CLIENT_PASSWORD { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        [DataMember]
        public string TSERVER_QUEUE { get; set; }

        /// <summary>
        /// 坐席分机号
        /// </summary>
        [DataMember]
        public string DN { get; set; }

        /// <summary>
        /// 当前员工编号
        /// </summary>
        [DataMember]
        public string AgentID { get; set; }

        /// <summary>
        /// 当前员工密码 默认空
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// 今日呼入量
        /// </summary>
        [DataMember]
        public int TodayCallInCount { get; set; }

        /// <summary>
        /// 今日呼出量
        /// </summary>
        [DataMember]
        public int TodayCallOutCount { get; set; }

        /// <summary>
        /// 今日chat接入量
        /// </summary>
        [DataMember]
        public int TodayChatInCount { get; set; }

        /// <summary>
        /// 今日voice秒
        /// </summary>
        [DataMember]
        public int AHT { get; set; }

        /// <summary>
        /// 外拨授权码
        /// </summary>
        [DataMember]
        public string AuthorizedNumber { get; set; }

        /// <summary>
        /// 坐席voice状态
        /// </summary>
        [DataMember]
        public AgentStatus AgentStatus { get; set; }
        #endregion

        #region
        /// <summary>
        /// 连接超时 毫秒
        /// </summary>
        public int ConnectionTimeOut { get; set; }
        #endregion

        #region
        /// <summary>
        /// 是否点击了退出按钮
        /// </summary>
        [DataMember]
        public bool Exit { get; set; }

        /// <summary>
        /// 连接是否丢失了
        /// </summary>
        [DataMember]
        public bool IsLinkDisconnected { get; set; }

        /// <summary>
        /// 连接是否打开
        /// </summary>
        [DataMember]
        public bool IsLinkConnected { get; set; }

        /// <summary>
        /// 1.是否注册了
        /// </summary>
        [DataMember]
        public bool IsRegistered { get; set; }

        /// <summary>
        /// 2.是否已经登录
        /// </summary>
        [DataMember]
        public bool IsAgentLogin { get; set; }

        /// <summary>
        /// 是否已经登出
        /// </summary>
        [DataMember]
        public bool IsAgentLogout { get; set; }

        /// <summary>
        /// 是否注销成功
        /// </summary>
        [DataMember]
        public bool IsUnregistered { get; set; }
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
        public string Bakup_TSERVER_URI { get; set; }
        #endregion
    }
}
