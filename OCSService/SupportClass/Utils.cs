using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCSService.SupportClass
{
    public class Utils
    {
        public static int tenantid = Convert.ToInt32(ConfigurationManager.AppSettings["tenantid"]);
        public static string switchname = ConfigurationManager.AppSettings["switchname"];

        public static string cfgserverhost = ConfigurationManager.AppSettings["cfgserverhost"];
        public static int cfgserverport = Convert.ToInt32(ConfigurationManager.AppSettings["cfgserverport"]);
        public static string sipserverhost = ConfigurationManager.AppSettings["sipserverhost"];
        public static int sipserverport = Convert.ToInt32(ConfigurationManager.AppSettings["sipserverport"]);
        public static string ocsserverhost = ConfigurationManager.AppSettings["ocsserverhost"];
        public static int ocsserverport = Convert.ToInt32(ConfigurationManager.AppSettings["ocsserverport"]);
        public static string statserverhost = ConfigurationManager.AppSettings["statserverhost"];
        public static int statserverport = Convert.ToInt32(ConfigurationManager.AppSettings["statserverport"]);
      
        
        public static string thisdn = ConfigurationManager.AppSettings["thisdn"];
        public static string agentname = ConfigurationManager.AppSettings["agentname"];

        public static string username = ConfigurationManager.AppSettings["username"];
        public static string password = ConfigurationManager.AppSettings["password"];

        public static string srctable = ConfigurationManager.AppSettings["srctable"];
        public static string dsttable = ConfigurationManager.AppSettings["dsttable"];
        public static string dburl = ConfigurationManager.AppSettings["dburl"];

        public static string statserverdbid = ConfigurationManager.AppSettings["statserverdbid"];

    }
}
