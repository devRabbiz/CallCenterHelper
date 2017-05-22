using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model.stat
{
    public class TransferChatAgent
    {
        public int DBID { get; set; }
        /// <summary>
        /// 坐席名称
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 用户名。大部分是P开头的
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string AgentId { get; set; }
        /// <summary>
        /// place
        /// </summary>
        public string PlaceId { get; set; }
        public bool IsReady { get; set; }
        public int CurrentChatCount { get; set; }
    }
}
