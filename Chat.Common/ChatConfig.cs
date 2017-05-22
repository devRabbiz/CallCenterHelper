using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Chat.CustomerInterface
{
    public static class ChatAppSettings
    {

        /// <summary>
        /// 主机
        /// </summary>
        public static string Host
        {
            get
            {
                string host = ConfigurationManager.AppSettings["chatHost"];
                if (string.IsNullOrEmpty(host))
                    throw new Exception("Please check chat host settings!");
                return host;
            }
        }

        /// <summary>
        /// 端口
        /// </summary>
        public static int Port
        {
            get
            {
                string port = ConfigurationManager.AppSettings["chatPort"];
                if (string.IsNullOrEmpty(port))
                    throw new Exception("Please check chat port settings!");
                return int.Parse(port);
            }
        }

        /// <summary>
        /// 应用名称
        /// </summary>
        public static string AppName
        {
            get
            {
                string name = ConfigurationManager.AppSettings["appName"];
                if (string.IsNullOrEmpty(name))
                    throw new Exception("Please check chat appName settings!");
                return name;
            }
        }



    }
}
