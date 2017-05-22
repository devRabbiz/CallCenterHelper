using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model.cfg
{
    public class TransferAgent
    {
        public int DBID { get; set; }
        public string AgentName { get; set; }
        public string DN { get; set; }
        public bool IsReady { get; set; }
    }
}
