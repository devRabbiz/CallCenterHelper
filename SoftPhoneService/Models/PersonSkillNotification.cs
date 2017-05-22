using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftPhoneService.Models
{
    /// <summary>
    /// 坐席所属队列修改通知
    /// </summary>
    public class PersonSkillNotification
    {
        public int skillDBID { get; set; }
        public int level { get; set; }
    }
}