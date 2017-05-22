using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model.cfg
{
    public class Skill
    {
        public int DBID { get; set; }
        public string Name { get; set; }

        public string ObjectPath { get; set; }

        /// <summary>
        /// Voice_Skill：Voice的队列， 属于@Switch_Avaya
        /// MM_Skill：Chat队列,属于@Switch_MM
        /// NJ_Skill：南京队列，属于@Switch_Avaya_NJ
        /// </summary>
        public string SwitchName { get; set; }

        /// <summary>
        /// 订阅用到 VQ231
        /// </summary>
        public string Number { get; set; }


        /// <summary>
        /// Stat服务最后更新
        /// </summary>
        public DateTime LastDate { get; set; }
        /// <summary>
        /// Stat服务最新排队量
        /// </summary>
        public int LastValue { get; set; }
        /// <summary>
        /// 订阅状态 0 未知,1 Opend,2 Closed
        /// </summary>
        public int StatisticStatus { get; set; }
    }
}
