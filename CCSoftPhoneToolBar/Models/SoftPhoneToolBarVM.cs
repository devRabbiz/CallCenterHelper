using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftPhoneToolBar.Models
{
    public class SoftPhoneToolBarVM
    {
        public string employee_id { get; set; }
        public string DN { get; set; }
        public string Place { get; set; }
        /// <summary>
        /// 0登录业务系统 1应急登录
        /// </summary>
        public int IsEmergency { get; set; }
        public string password { get; set; }
    }
}