using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Genesys.cfg
{
    public partial class Proc_GetPersonAgentInfo_Result
    {
        public Nullable<int> person_dbid { get; set; }
        public string login_code { get; set; }
        public Nullable<int> login_dbid { get; set; }
        public string employee_id { get; set; }
        public string first_name { get; set; }
        public string user_name { get; set; }
        public Nullable<int> chat { get; set; }
    }
}
