using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SoftPhoneService.SupportClass
{

    [Obsolete("", true)]
    public class LogHelper
    {
        public class LogInfo
        {
            public string type { get; set; }
            public string message { get; set; }
            public string category { get; set; }
            public DateTime datetime { get; set; }
        }
        private static object LOCKOBJ = new object();

        private static Queue<LogInfo> queue = new Queue<LogInfo>();

        private static string FILEPATH = string.Empty;

        static LogHelper()
        {
            var now = DateTime.Now;
            FILEPATH = HttpContext.Current.Server.MapPath("~/Log/" + now.Year + "/" + now.Month + "/" + now.Month.ToString().PadLeft(2, '0') + "-" + now.Day.ToString().PadLeft(2, '0') + ".log");
            var path = Path.GetDirectoryName(FILEPATH);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void LogMessage(string msg, string category = "")
        {
            Log("Message", msg, category);
        }

        public static void LogRequest(string msg, string category = "")
        {
            Log("Request", msg, category);
        }

        public static void LogResponse(string msg, string category = "")
        {
            Log("Response", msg, category);
        }

        public static void LogException(Exception e, string category = "")
        {
            Log("Exception", e.ToString(), category);
        }

        private static void Log(string type, string message, string category = "")
        {
            queue.Enqueue(new LogInfo() { type = type, message = message, category = category, datetime = DateTime.Now });
            if (queue.Count > 50)
            {
                WriteToLogFile();
            }
        }

        public static void End()
        {
            WriteToLogFile();
        }

        private static void WriteToLogFile()
        {
            lock (LOCKOBJ)
            {
                using (var sw = new StreamWriter(FILEPATH, true, System.Text.Encoding.UTF8))
                {
                    var qcount = queue.Count;
                    for (var i = 0; i < qcount; i++)
                    {
                        var info = queue.Dequeue();
                        if (info.category != "")
                        {
                            info.category = "[" + info.category + "]";
                        }
                        sw.WriteLine(string.Format("{0}{1}   {2}", info.category, info.type, info.datetime));
                        sw.WriteLine(info.message);
                        sw.WriteLine("********************");
                        sw.WriteLine("");
                    }
                }
            }
        }

    }


}