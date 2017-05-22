using Genesyslab.Platform.Commons.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VoiceOCX.Models
{
    /// <summary>
    /// 交互状态
    /// </summary>
    public enum InteractionState
    {
        Alerting,
        Accepted
    }

    /// <summary>
    /// 交互
    /// </summary>
    [Serializable]
    [DataContract]
    public class Interaction
    {
        public Interaction()
        {
            UserData = new Dictionary<string, string>();
        }

        /// <summary>
        /// 交互状态
        /// </summary>
        [DataMember]
        public InteractionState State { get; set; }

        /// <summary>
        /// 票据ID
        /// </summary>
        [DataMember]
        public int TicketId { get; set; }

        /// <summary>
        /// 交互ID
        /// </summary>
        [DataMember]
        public string InteractionId { get; set; }

        /// <summary>
        /// 随路数据
        /// </summary>
        [DataMember]
        public Dictionary<string, string> UserData { get; set; }
    }

}
