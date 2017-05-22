using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model.stat
{
    public class StatisticItem : ICloneable
    {
        public StatisticItem()
        {
            LastDate = DateTime.Now;
        }
        public int DBID { get; set; }
        public string ObjectId { get; set; }

        #region 扩展
        /// <summary>
        /// 通过父BaseReferenceId+DBID，已经形成唯一ID
        /// </summary>
        public int ReferenceId { get; set; }
        /// <summary>
        /// 打开订阅成功后保存到这里;关闭订阅时用这个
        /// </summary>
        public int REQ_ID { get; set; }
        public Statistic Statistic { get; set; }

        /// <summary>
        /// 事件返回是否关闭
        /// </summary>
        public bool Closed { get; set; }
        /// <summary>
        /// 事件返回是否打开
        /// </summary>
        public bool Opened { get; set; }

        /// <summary>
        /// 坐席要求打开订阅（首次登录：Opend=false并且RequireOpen=true -> 打开订阅 -> 订阅成功:Opend=true）
        /// </summary>
        public bool RequireOpen { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastDate { get; set; }

        /// <summary>
        /// 最新值 （如果是坐席状态:4为就绪 其他非就绪）
        /// </summary>
        public int LastValue { get; set; }

        /// <summary>
        /// 区分坐席和队列:skill=1 agent=2
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// chat坐席正在接起的数
        /// </summary>
        public int CurrentChatCount { get; set; }
        #endregion

        public object Clone()
        {
            var other = (StatisticItem)this.MemberwiseClone();
            other.Statistic = (Statistic)(this.Statistic.Clone());
            return other;
        }
    }
}
