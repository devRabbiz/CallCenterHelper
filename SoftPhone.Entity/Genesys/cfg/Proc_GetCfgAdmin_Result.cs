using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Genesys.cfg
{
    public partial class Proc_GetCfgAdmin_Result
    {
        public decimal dbid { get; set; }
        public Nullable<decimal> tenant_dbid { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string address_line3 { get; set; }
        public string address_line4 { get; set; }
        public string address_line5 { get; set; }
        public string office { get; set; }
        public string home { get; set; }
        public string mobile { get; set; }
        public string pager { get; set; }
        public string fax { get; set; }
        public string modem { get; set; }
        public string phones_comment { get; set; }
        public string birthdate { get; set; }
        public string comment_ { get; set; }
        public string employee_id { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public Nullable<int> is_agent { get; set; }
        public Nullable<int> is_admin { get; set; }
        public Nullable<int> state { get; set; }
        public Nullable<decimal> csid { get; set; }
        public Nullable<decimal> tenant_csid { get; set; }
        public Nullable<decimal> place_dbid { get; set; }
        public Nullable<decimal> place_csid { get; set; }
        public Nullable<decimal> capacity_dbid { get; set; }
        public Nullable<decimal> site_dbid { get; set; }
        public Nullable<decimal> contract_dbid { get; set; }
    }
}
