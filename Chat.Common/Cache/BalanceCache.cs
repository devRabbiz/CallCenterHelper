using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.IO;
using System.Web.UI;
using System.Xml.Linq;

namespace Chat.Common
{
    public class BalanceCache
    {
        #region Constructors

        private static BalanceCache Instance = null;
        private static object LOCK_INDEX = new object();
        private static int CURRENT_INDEX = 0;
        private static string CONFIG_FILE_NAME = string.Empty;

        private BalanceCache()
        {
            CONFIG_FILE_NAME = string.Format("{0}Content\\ChatServerInfo.xml", HttpContext.Current.Request.PhysicalApplicationPath);
        }

        public static BalanceCache GetInstance()
        {
            if (BalanceCache.Instance == null) BalanceCache.Instance = new BalanceCache();
            return BalanceCache.Instance;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 获取ChatServer信息
        /// </summary>
        /// <returns></returns>
        public ChatServerInfo GetServiceInfo()
        {
            Cache cache = HttpContext.Current.Cache;
            List<ChatServerInfo> data = cache["ChatServerInfo"] as List<ChatServerInfo>;
            if (data == null || data.Count == 0)
            {
                CacheDependency fileDependency = new CacheDependency(CONFIG_FILE_NAME);
                data = GetChatServerInfos();
                cache.Insert("ChatServerInfo", data, fileDependency);
            }
            if (data != null && data.Count > 0)
            {
                lock (LOCK_INDEX)
                {
                    CURRENT_INDEX++;
                    if (CURRENT_INDEX >= data.Count) CURRENT_INDEX = 0;
                }
                return data[CURRENT_INDEX];
            }
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 获取菜单组集合
        /// </summary>
        /// <param name="moduleID">模块ID</param>
        /// <returns></returns>
        private static List<ChatServerInfo> GetChatServerInfos()
        {
            List<ChatServerInfo> servers = new List<ChatServerInfo>();
            IEnumerable<XElement> lst = LoadAllElement();
            foreach (XElement ele in lst)
            {
                ChatServerInfo info = new ChatServerInfo(ele);
                servers.Add(info);
            }
            return servers;
        }

        private static IEnumerable<XElement> LoadAllElement()
        {
            IEnumerable<XElement> result = null;
            try
            {
                XDocument doc = XDocument.Load(CONFIG_FILE_NAME, LoadOptions.None);
                result = doc.Element("chatServers").Elements("chatServer")
                    .Where(e => IsValidServer(e.Attribute("enable")));
            }
            catch { }
            return result;
        }

        private static bool IsValidServer(XAttribute arr)
        {
            return arr != null && arr.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion

    }

    public class ChatServerInfo
    {
        #region Properties

        public string Alias { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public Uri Uri
        {
            get
            {
                return new Uri(string.Format("tcp://{0}:{1}", this.Host, this.Port));
            }
        }

        #endregion

        #region Constructors

        public ChatServerInfo() { }

        public ChatServerInfo(XElement element)
        {
            this.Alias = GetValidValue(element.Element("alias"));
            this.Host = GetValidValue(element.Element("host"));
            string port = GetValidValue(element.Element("port"));
            try
            {
                this.Port = Convert.ToInt32(port);
            }
            catch { }
        }
        #endregion

        #region Private Methods

        public static string GetValidValue(XElement ele)
        {
            return GetValidValue(ele, string.Empty);
        }

        public static string GetValidValue(XElement ele, string defaultValue)
        {
            string reslut = string.Empty;
            if (ele == null || string.IsNullOrEmpty(ele.Value.Trim()))
            {
                reslut = defaultValue;
            }
            else
            {
                reslut = ele.Value.Trim();
            }
            return reslut;
        }

        #endregion

    }


}