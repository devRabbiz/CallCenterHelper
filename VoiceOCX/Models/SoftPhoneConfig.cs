using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VoiceOCX.Models
{
    [Serializable]
    [DataContract]
    public class SoftPhoneConfig
    {
        public SoftPhoneConfig()
        {
            TServerConfig = new TServerConfig();
            IServerConfig = new IServerConfig();
        }

        /// <summary>
        /// 呼叫中心代码: BJ NJ
        /// </summary>
        [DataMember]
        public string CallCenter { get; set; }

        /// <summary>
        /// 电话服务配置
        /// </summary>
        [DataMember]
        public TServerConfig TServerConfig { get; set; }

        /// <summary>
        /// Chat排队服务配置
        /// </summary>
        [DataMember]
        public IServerConfig IServerConfig { get; set; }

        [DataMember]
        public bool LogMessage { get; set; }

        [DataMember]
        public bool LogResponse { get; set; }

        [DataMember]
        public bool LogRequest { get; set; }

        [DataMember]
        public bool LogException { get; set; }

        [DataMember]
        public bool LogTrace { get; set; }
    }
}
