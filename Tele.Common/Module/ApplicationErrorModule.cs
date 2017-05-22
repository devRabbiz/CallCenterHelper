using System;
using System.IO;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Configuration;
using System.Web.Configuration;
using System.Collections.Generic;

namespace Tele.Common
{
    public class ApplicationErrorModule : IHttpModule
    {
        public static readonly object ExceptionItemKey = new object();

        void IHttpModule.Init(HttpApplication context)
        {
            context.Error += new EventHandler(context_Error);
        }

        void IHttpModule.Dispose()
        {
        }

        #region context_Error

        private void context_Error(object sender, EventArgs e)
        {
            if (!(HttpContext.Current.CurrentHandler is Page
                || HttpContext.Current.Request.FilePath.EndsWith("aspx", StringComparison.OrdinalIgnoreCase)
                || HttpContext.Current.Request.FilePath.EndsWith("ashx", StringComparison.OrdinalIgnoreCase)))
                return;

            Page page = HttpContext.Current.CurrentHandler as Page;
            if (page == null || !page.IsCallback)
            {
                string errorPageUrl = string.Empty;
                HttpContext context = HttpContext.Current;
                if (context.Error != null)
                {
                    ScriptManager sm = null;
                    if (page != null) sm = ScriptManager.GetCurrent(page);
                    if (sm == null || !sm.IsInAsyncPostBack)
                    {
                        string stackTrace = GetAllStackTrace(context.Error);
                        Exception realEx = Utils.GetRealException(context.Error);

                        context.ClearError();
                        TryWriteLog(realEx, stackTrace);

                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.Write(realEx.Message);
                        HttpContext.Current.Response.End();
                    }
                }
            }
        }


        private static string GetAllStackTrace(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            for (Exception innerEx = ex; innerEx != null; innerEx = innerEx.InnerException)
            {
                if (sb.Length > 0) sb.Append("\n");
                sb.AppendFormat("-------------{0}：{1}-----------\r\n", innerEx.GetType().Name, innerEx.Message);
                sb.Append(innerEx.StackTrace);
            }
            return sb.ToString();
        }

        private static void TryWriteLog(Exception ex, string stackTrace)
        {
            try
            {
                string fileName = string.Format(@"{0}logs\app_{1:yyyyMM}\{1:yyyyMMdd}.log"
                    , AppDomain.CurrentDomain.BaseDirectory, DateTime.Now);
                HttpContext context = HttpContext.Current;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("异常信息：{0} ，源:{1}，时间：{2}\r\n\r\n", ex.Message, ex.Source, DateTime.Now);

                sb.AppendFormat("客户IP地址:{0}\r\n", context.Request.UserHostAddress);
                sb.AppendFormat("请求地址:{0}\r\n", context.Request.Url.AbsoluteUri);
                sb.AppendFormat("堆栈信息:{0}\r\n", stackTrace);

                ChatHelper.WriteFile(fileName, sb.ToString());
            }
            catch { }
        }

        #endregion

    }
}
