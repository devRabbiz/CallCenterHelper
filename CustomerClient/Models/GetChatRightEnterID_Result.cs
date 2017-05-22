using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerClient.Models
{
    public class GetChatRightEnterID_Result
    {
        public string Return { get; set; }
        /// <summary>
        /// 1队列 2找坐席
        /// </summary>
        public int Type { get; set; }
    }
}