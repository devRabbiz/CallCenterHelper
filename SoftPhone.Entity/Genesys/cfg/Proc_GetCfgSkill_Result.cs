using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Genesys.cfg
{
    public partial class Proc_GetCfgSkill_Result
    {
        public decimal dbid { get; set; }
        public string name { get; set; }
        public Nullable<decimal> tenant_dbid { get; set; }
        public Nullable<int> state { get; set; }
        public Nullable<decimal> csid { get; set; }
        public Nullable<decimal> tenant_csid { get; set; }
    }
}
