using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftPhoneToolBar.Models
{
    public class InitInfoVM
    {
        public int ErrCode { get; set; }
        public string ErrMessage { get; set; }

        public string DN { get; set; }
        public string Place { get; set; }
        public string AgentID { get; set; }
        public string FirstName { get; set; }
        public string EmployeeID { get; set; }
        public bool EnableVoice { get; set; }
        public bool EnableChat { get; set; }

        public SoftPhone.Entity.Model.cfg.Person Person { get; set; }
    }
}