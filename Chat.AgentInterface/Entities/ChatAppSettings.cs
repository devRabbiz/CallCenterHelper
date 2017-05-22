//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Configuration;

//namespace Chat.AgentInterface
//{
//    public static class ChatAppSettings
//    {

//        /// <summary>
//        /// 聊天服务器地址配置
//        /// </summary>
//        public static string ChatServer
//        {
//            get
//            {
//                string chatServer = ConfigurationManager.AppSettings["chatServer"];
//                if (string.IsNullOrEmpty(chatServer))
//                    throw new Exception("Please check chatServer settings!");
//                return chatServer;
//            }
//        }

//        /// <summary>
//        /// Tenant Id
//        /// </summary>
//        public static int TenantId
//        {
//            get
//            {
//                string tenantId = ConfigurationManager.AppSettings["tenantId"];
//                if (string.IsNullOrEmpty(tenantId))
//                    throw new Exception("Please check tenantId settings!");
//                return int.Parse(tenantId);
//            }
//        }



//    }
//}
