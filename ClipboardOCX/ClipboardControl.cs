using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using mshtml;
using System.IO;
using System.Net;
using CSharpWin;

namespace ClipboardOCX
{
    [Guid("EFD2F97C-3A62-4B31-A7F1-FBC5B8C64DCC")]
    public partial class ClipboardControl : UserControl, IObjectSafety
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

                }
            }
            catch { }
        }
        #endregion

        public ClipboardControl()
        {
            InitializeComponent();
        }

        private string _upload(Image img, string UploadPath)
        {
            var filename = "截图" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
            var dir = @"c:\cc\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            img.Save(dir + filename, System.Drawing.Imaging.ImageFormat.Jpeg);

            WebClient client = new WebClient();
            client.Headers["ocx"] = "agent";
            var buffer = client.UploadFile(UploadPath, "POST", dir + filename);
            var result = Encoding.GetEncoding("utf-8").GetString(buffer);
            return result;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="ID">tabid</param>
        /// <param name="UploadPath">上传入口</param>
        /// <returns></returns>
        public string UploadImg(string UploadPath)
        {
            try
            {
                if (Clipboard.ContainsImage())
                {
                    var img = Clipboard.GetImage();
                    return _upload(img, UploadPath);
                }
            }
            catch (Exception ex)
            {
                CallJavaScript("ClipboardOCX_ERROR", ex.Message);
            }
            return string.Empty;
        }

        //截图
        public string CaptureImage()
        {
            var r = "";
            try
            {
                CaptureImageTool capture = new CaptureImageTool();
                if (capture.ShowDialog() == DialogResult.OK)
                {
                    var img = capture.Image;
                    Clipboard.SetImage(img);
                    r = "1:截图成功";
                }
                else
                {
                    r = "1:取消截图";
                }
            }
            catch (Exception ex)
            {
                return "0:截图失败:" + ex.Message;
            }
            return r;
        }
    }
}
