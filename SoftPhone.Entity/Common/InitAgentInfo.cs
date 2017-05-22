using SoftPhone.Entity.Model.cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Common
{
    public class InitAgentInfo
    {
        public InitAgentInfo()
        {
            this.SkillLevels = new List<SkillLevel>();
        }
        public int ErrCode { get; set; }
        public string ErrMessage { get; set; }

        public string DN { get; set; }
        public string Place { get; set; }

        public int PersonDBID { get; set; }
        /// <summary>
        /// login_code
        /// </summary>
        public string AgentID { get; set; }
        public string FirstName { get; set; }
        public string EmployeeID { get; set; }
        public bool EnableVoice { get; set; }
        public bool EnableChat { get; set; }

        public List<SkillLevel> SkillLevels { get; set; }


    }
}
