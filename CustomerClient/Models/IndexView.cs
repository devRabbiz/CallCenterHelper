using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chat.Common;

namespace CustomerClient.Models
{
    public class IndexView
    {
        /// <summary>
        /// 当前客服
        /// </summary>
        public string CurrentAgentNames { get { return "姜子牙；貂蝉"; } }

        /// <summary>
        /// 用户数据
        /// </summary>
        public string ClientUserData { get; set; }

        /// <summary>
        /// 当前队列名称
        /// </summary>
        public string CurrentQueueName { get; set; }

        /// <summary>
        /// 访客提问机器人的问题
        /// </summary>
        public string RabitPreText { get; set; }

    }
}