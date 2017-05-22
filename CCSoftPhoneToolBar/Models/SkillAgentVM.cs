using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftPhoneToolBar.Models
{
    public class SkillAgentVM
    {
        /// <summary>
        /// cfg_person.dbid
        /// </summary>
        public decimal ID { get; set; }
        public string EmployeeID { get; set; }
        public string AgentName { get; set; }
        /// <summary>
        /// 就绪 未就绪
        /// </summary>
        public bool IsReady { get; set; }
        public string DN { get; set; }
    }
}