/*
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Collections;
using System.Runtime.Serialization;
using Genesyslab.Platform.Voice.Protocols.TServer;
using System.Collections.Specialized;
namespace VoiceOCX.Models
{
    [Serializable]
    [DataContract]
    public class VoiceFlow
    {
        public VoiceFlow()
        {
            UserData = new Dictionary<string, string>();
        }

        /// <summary>
        /// 员工编号+yyyyMMddHHmmss
        /// </summary>
        [DataMember]
        public string CallID { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        [DataMember]
        public string CustomerID { get; set; }

        /// <summary>
        /// 当前电话的InteractionID
        /// </summary>
        [DataMember]
        public string InteractionID { get; set; }

        /// <summary>
        /// 当前坐席状态
        /// </summary>
        [DataMember]
        public AgentStatus AgentStatus { get; set; }

        /// <summary>
        /// 当前通话类型:Unknown 未知,Internal 内线,Inbound 呼入,Outbound 外拨,Consult 咨询
        /// </summary>
        [DataMember]
        public CallType CallType { get; set; }

        /// <summary>
        /// 当前电话的0线 ConnectionID
        /// </summary>
        [DataMember]
        public string ConnectionID { get; set; }

        /// <summary>
        /// 需要转的ConnectionID.因为从路由过来是muteTransefer
        /// </summary>
        [DataMember]
        public string TransferConnectionID { get; set; }

        /// <summary>
        /// 哪个connid触发的
        /// </summary>
        [DataMember]
        public string CurrentConnectionID { get; set; }

        /// <summary>
        /// 当前电话的1线 ConnectionID
        /// </summary>
        [DataMember]
        public string SecondConnectionID { get; set; }

        /// <summary>
        /// 电话来自的队列
        /// </summary>
        [DataMember]
        public string AgentgroupName { get; set; }

        /// <summary>
        /// 前一个坐席组
        /// </summary>
        [DataMember]
        public string PreAgentgroupName { get; set; }

        /// <summary>
        /// 如果要转接出去的话，代表后一个坐席组
        /// </summary>
        [DataMember]
        public string NextAgentgroupName { get; set; }

        /// <summary>
        /// 转接过来的话，获得前一个坐席
        /// </summary>
        [DataMember]
        public string PreAgentId { get; set; }

        /// <summary>
        /// 转接出去的话，代表下一个坐席
        /// </summary>
        [DataMember]
        public string NextAgentId { get; set; }

        /// <summary>
        /// 当前ANI：主叫号码
        /// </summary>
        [DataMember]
        public string ANI { get; set; }

        /// <summary>
        /// 被叫号码：外线是热线，内线是分机
        /// </summary>
        [DataMember]
        public string DNIS { get; set; }

        /// <summary>
        /// 哪个分机号参与了这一请求或事件
        /// </summary>
        [DataMember]
        public string ThisDN { get; set; }

        /// <summary>
        /// 随路数据
        /// </summary>
        [DataMember]
        public Dictionary<string, string> UserData { get; set; }

        /// <summary>
        /// 当前队列名称
        /// </summary>
        [DataMember]
        public string CurrentQueueName { get; set; }

        /// <summary>
        /// 转入队列名称
        /// </summary>
        [DataMember]
        public string FromQueueName { get; set; }

        /// <summary>
        /// 转出队列名称
        /// </summary>
        [DataMember]
        public string NextQueueName { get; set; }

        /// <summary>
        /// 1：会议
        /// </summary>
        [DataMember]
        public int IsConference { get; set; }

        /// <summary>
        /// 0：默认没有转接 1：转接至队列 2：转接至坐席 3：转接至IVR 
        /// </summary>
        [DataMember]
        public int IsTransfer { get; set; }

        /// <summary>
        /// 1：转接至电话支付
        /// </summary>
        [DataMember]
        public int IsTransferEPOS { get; set; }

        /// <summary>
        /// 1:呼入 2：预览呼出
        /// </summary>
        [DataMember]
        public int InOut { get; set; }

        /// <summary>
        /// 电话支付下：第三方的DN。如果包含在ANI或DNIS里能找到则说明用户挂断。（提示坐席，并且挂断0线）
        /// </summary>
        [DataMember]
        public string ThirdPartyDN { get; set; }

        /// <summary>
        /// Internal，需要显示主叫坐席分机
        /// </summary>
        [DataMember]
        public string OtherDN { get; set; }
    }
}
