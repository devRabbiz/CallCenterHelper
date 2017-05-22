using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftPhoneToolBar.Models
{
    public class LoginVM
    {
        public LoginVM()
        {
            Items = new List<SelectListItem>();
            Items.Add(new SelectListItem() { Text = "登录电话应急系统", Value = "1" });
            Items.Add(new SelectListItem() { Text = "登录电话管理系统", Value = "2" });
        }

        [Required]
        [Display(Name = "员工编号")]
        public string employee_id { get; set; }
        [Display(Name = "密码")]
        public string password { get; set; }
        [Display(Name = "登录系统")]
        public int IsEmergency { get; set; }

        public List<SelectListItem> Items { get; set; }

        public string DN { get; set; }
        public string Place { get; set; }
    }
}