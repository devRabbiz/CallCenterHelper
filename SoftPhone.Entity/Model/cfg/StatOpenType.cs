using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model.cfg
{
    /// <summary>
    /// 订阅类型
    /// </summary>
    public enum StatOpenType
    {
        /// <summary>
        /// 队列排队量
        /// </summary>
        Open11_Current_In_Queue = 11,
        /// <summary>
        /// 呼入量
        /// </summary>
        Open12_TotalNumberInboundCalls = 12,
        /// <summary>
        /// 呼出量
        /// </summary>
        Open13_TotalNumberOutboundCalls = 13,
        /// <summary>
        /// Chat处理量
        /// </summary>
        Open14_Total_Inbound_Handled = 14,
        /// <summary>
        /// 呼出AHT
        /// </summary>
        [Obsolete]
        Open15_AverOutboundStatusTime = 15,
        /// <summary>
        /// 案面时长
        /// </summary>
        Open16_Total_Work_Time = 16,
        /// <summary>
        /// 通话时长
        /// </summary>
        Open17_Total_Talk_Time_CC = 17,
        /// <summary>
        /// 坐席状态
        /// </summary>
        Open18_CurrentAgentState = 18
    }
}
