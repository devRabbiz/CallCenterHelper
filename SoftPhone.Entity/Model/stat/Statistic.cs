using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model.stat
{
    public class Statistic : ICloneable
    {
        public Statistic()
        {
        }

        /// <summary>
        /// 统计的类型：skill/MM, agent/12 agent/13 ... agent/19
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// 队列排队量：    Current_In_Queue
        /// 呼入量：        TotalNumberInboundCalls
        /// 呼出量：        TotalNumberOutboundCalls
        /// Chat处理总量：  Total_Inbound_Handled
        /// 呼入AHT：（案面时长+通话时长）/呼入数量
        ///                 (Total_Work_Time + Total_Talk_Time_CC)/TotalNumberInboundCalls
        /// 坐席状态：      CurrentAgentState
        /// CHAT坐席状态：  CurrentAgentState
        /// </summary>
        public string StatisticType { get; set; }

        /// <summary>
        /// Agent = 0, AgentPlace = 1, Queue = 5, RegularDN = 8,
        /// </summary>
        public int ObjectType { get; set; }

        /// <summary>
        /// tcp://10.99.36.253:3490
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// 根据URI为主
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 序号 系统自增
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 基础引用号段
        /// </summary>
        public int BaseReferenceId { get; set; }

        public object Clone()
        {
            var other = (Statistic)this.MemberwiseClone();
            return other;
        }
    }
}
