using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model.cfg
{
    public class AgentStatisticResult
    {
        /// <summary>
        /// 主队列量
        /// </summary>
        public int QueueCount1 { get; set; }
        /// <summary>
        /// 辅队列量
        /// </summary>
        public int QueueCount2 { get; set; }

        /// <summary>
        /// callin
        /// </summary>
        public int CallInCount { get; set; }
        /// <summary>
        /// callout
        /// </summary>
        public int CallOutCount { get; set; }
        /// <summary>
        /// Chat量
        /// </summary>
        public int ChatInCount { get; set; }

        /// <summary>
        /// 呼入AHT
        /// </summary>
        public int AHT { get; set; }
    }
}
