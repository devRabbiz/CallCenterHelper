using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using mshtml;
using Genesyslab.Platform.Commons.Protocols;
using System.Threading;

namespace VoiceOCX
{
    public partial class SoftPhone
    {
        #region IObjectSafety
        public void GetInterfacceSafyOptions(Int32 riid, out Int32 pdwSupportedOptions, out Int32 pdwEnabledOptions)
        {
            pdwSupportedOptions = CLsObjectSafety.INTERFACESAFE_FOR_UNTRUSTED_CALLER;
            pdwEnabledOptions = CLsObjectSafety.INTERFACESAFE_FOR_UNTRUSTED_DATA;
        }

        public void SetInterfaceSafetyOptions(Int32 riid, Int32 dwOptionsSetMask, Int32 dwEnabledOptions)
        {

        }
        #endregion

        #region Javascript
        private void CallJavaScript(string callFunctionName, params object[] args)
        {
            try
            {
                Type typeIOleObject = this.GetType().GetInterface("IOleObject", true);
                object oleClientSite = typeIOleObject.InvokeMember("GetClientSite",
                 BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
                null,
                this,
                null);

                IOleClientSite oleClientSite2 = oleClientSite as IOleClientSite;
                IOleContainer pObj;
                oleClientSite2.GetContainer(out pObj);

                IHTMLDocument pDoc2 = (IHTMLDocument)pObj;
                object script = pDoc2.Script;

                try
                {
                    script.GetType().InvokeMember(callFunctionName,
                    BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
                   null,
                    script,
                    args);
                }
                catch
                {
                    LogException(new Exception("1.未实现JS方法：" + callFunctionName + ";2.在调用该方法时发生内部错误"));
                }
            }
            catch { }
        }

        #region 记录日志
        private void LogMessage(string message)
        {
            LogTrace(message);
            if (cfg.LogMessage)
            {
                CallJavaScript("LogMessage", message);
            }
        }
        private void LogRequest(string request)
        {
            LogTrace(request);
            if (cfg.LogRequest)
            {
                CallJavaScript("LogRequest", request);
            }
        }
        private void LogException(Exception e)
        {
            try
            {
                eventLog1.WriteEntry(DateTime.Now.ToString() + "\r\n" + e.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
            catch { }
            CallJavaScript("LogException", e.ToString());
        }
        private void LogResponse(string response)
        {
            LogTrace(response);
            if (cfg.LogResponse)
            {
                CallJavaScript("LogResponse", response);
            }
        }
        private void LogTrace(string message)
        {
            try
            {
                if (cfg.LogTrace)
                {
                    eventLog1.WriteEntry(DateTime.Now.Ticks + ":" + message, System.Diagnostics.EventLogEntryType.Information);
                }
            }
            catch { }
        }
        #endregion

        #region
        /// <summary>
        /// 1为TS 2为IS
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        private void OcxError(Exception err, int type)
        {
            CallJavaScript("OcxError", err.Message, err.ToString(), type);
        }
        #endregion
        #endregion
    }
}
