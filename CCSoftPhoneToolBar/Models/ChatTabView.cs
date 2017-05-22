using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftPhoneToolBar.Models
{
    public class ChatTabView
    {
        public string TicketID { get; set; }

        public string CurrentSessionID { get; set; }

        public string CustomerID { get; set; }

        public string CustomerName { get; set; }

        public string MachineNO { get; set; }

        /// <summary>
        /// 当前队列名称
        /// </summary>
        public string CurrentQueueName { get; set; }

        /// <summary>
        /// 访客连接的ChatServer主机名
        /// </summary>
        public string ChatServerHost { get; set; }

        /// <summary>
        /// 访客连接的ChatServer端口
        /// </summary>
        public string ChatServerPort { get; set; }


        public string ChatData { get; set; }

        public string CustomerIP { get; set; }

    }



    /// <summary>
    /// 随路数据
    /// Chat.AgentInterface
    /// </summary>
    public class CurrentDataReference
    {
        public string EnterID { get; set; }
        public string CustomerID { get; set; }
        public string FirstName { get; set; }
        public string CustomerName { get; set; }
        public string MachineNO { get; set; }
        public string MailAddress { get; set; }
        public string CurrentQueue { get; set; }
        public string FromQueue { get; set; }
        public string NextQueue { get; set; }

        public int IsTransfer { get; set; }
        public int IsRTO { get; set; }
        public int IsMeeting { get; set; }

        public string CustomerIP { get; set; }

        /// <summary>
        /// 访客连接的ChatServer主机名
        /// </summary>
        public string ChatServerHost { get; set; }

        /// <summary>
        /// 访客连接的ChatServer端口
        /// </summary>
        public string ChatServerPort { get; set; }

    }

}