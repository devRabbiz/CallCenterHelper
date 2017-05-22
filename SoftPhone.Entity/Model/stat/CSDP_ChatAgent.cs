using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model.stat
{
    public class CSDP_ChatAgent
    {
        public int DBID { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// 是否就绪
        /// </summary>
        public bool IsReady { get; set; }

        /// <summary>
        /// 当前聊天数
        /// </summary>
        public int CurrentChatCount { get; set; }

        /// <summary>
        /// 排队量
        /// </summary>
        public int Place_In_Quene { get; set; }

        /// <summary>
        /// 如果DN为空则说明退出了
        /// </summary>
        public string DN { get; set; }

        /// <summary>
        /// 1:就绪 2:未就绪 3:离线
        /// </summary>
        public int Status { get; set; }
    }

    /// <summary>
    /// 用户队列
    /// </summary>
    public class CSDP_ChatAgentQuene
    {
        /// <summary>
        /// 用户队列
        /// </summary>
        public CSDP_ChatAgentQuene()
        {

        }
        public string VQ { get; set; }
        public string AgentId { get; set; }
    }

    public class CSDP_ChatAgentQuene_AllReady_Result
    {
        public string AgentId { get; set; }
    }
}
