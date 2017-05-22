using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using Tele.Common;

namespace Chat.Common
{
    public static class ChatHelper
    {
        public static DateTime AppendTicks(string ticks, DateTime defaultValue)
        {
            DateTime result = defaultValue;
            long mlsecond = 0;
            if (Int64.TryParse(ticks, out mlsecond))
                result = DateTime.MinValue.AddTicks(mlsecond);
            return result;
        }

        // 检查路径
        public static void CheckPath(string path)
        {
            if (!Path.IsPathRooted(path))
                path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + path;
            path = path.Replace('/', '\\').Replace("\\\\", "\\");
            if (path.Length > 7 && path.IndexOf(".", path.Length - 7) != -1)
                path = path.Substring(0, path.LastIndexOf('\\'));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 格式化文件大小 
        /// </summary>
        /// <param name="filesize"></param>
        /// <returns></returns>
        public static string FormatFileSize(long fileSize)
        {
            double maxSize = 1000 * 1024;
            double maxSize2 = 1024 * 1024;
            if (fileSize < 0)
                throw new ArgumentOutOfRangeException("fileSize");
            else if (fileSize >= maxSize * 1000) //文件大小大于或等于1024MB     
                return string.Format("{0:0.00} GB", (double)fileSize / (maxSize2 * 1024));
            else if (fileSize >= maxSize) //文件大小大于或等于1024KB     
                return string.Format("{0:0.00} MB", (double)fileSize / maxSize2);
            else
                return string.Format("{0:0.00} KB", (double)fileSize / 1024);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public static UploadModel UploadFile(HttpPostedFileBase file)
        {
            UploadModel model = new UploadModel();
            List<string> enabledTypes = new List<string>() { "image/pjpeg", "image/x-png", "image/bmp", "image/png", "image/jpeg" };

            //model.FileName = string.Format("{0}.jpg{1}", file.ContentLength, Path.GetExtension(file.FileName));
            model.FileName = string.Format("{0}.jpg", file.ContentLength, Path.GetExtension(file.FileName));

            model.FileSize = ChatHelper.FormatFileSize(file.ContentLength);
            model.ReturnFlag = false;

            //if (!enabledTypes.Exists(type => type == file.ContentType))
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!".jpg|.png|.bmp|.jpeg".Split('|').ToList().Exists(type => type == fileExtension))
            {
                model.Message = "上传失败：不支持的文件格式。（仅支持.jpg|.png|.bmp格式）";
            }
            else
            {
                string fileServer = System.Configuration.ConfigurationManager.AppSettings["fileServer"];
                string uploadPath = System.Configuration.ConfigurationManager.AppSettings["uploadPath"];
                string rootFolder = HttpContext.Current.Request.PhysicalApplicationPath;
                string fileName = string.Format("{0}{1}{2:yyyyMM}/{2:yyyyMMdd}/{2:HHmmss}_{3}",
                                    rootFolder, uploadPath, DateTime.Now, model.FileName);
                try
                {
                    ChatHelper.CheckPath(fileName);
                    file.SaveAs(fileName);
                    model.FileName = Path.GetFileName(file.FileName);
                    model.FileUrl = fileName.Replace(rootFolder, fileServer);
                    model.ReturnFlag = true;
                    model.Message = "上传成功。";
                }
                catch (Exception ex)
                {
                    model.Message = ex.Message;
                }
            }
            return model;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public static UploadModel UploadFile(HttpPostedFileBase file, UNCConfig cfg)
        {
            UploadModel model = new UploadModel();
            List<string> enabledTypes = new List<string>() { "image/pjpeg", "image/x-png", "image/bmp", "image/png", "image/jpeg" };

            //model.FileName = string.Format("{0}.jpg{1}", file.ContentLength, Path.GetExtension(file.FileName));
            model.FileName = string.Format("{0}.jpg", file.ContentLength, Path.GetExtension(file.FileName));

            model.FileSize = ChatHelper.FormatFileSize(file.ContentLength);
            model.ReturnFlag = false;

            if (!enabledTypes.Exists(type => type == file.ContentType))
            {
                model.Message = "上传失败：不支持的文件格式。（仅支持.jpg|.png|.bmp格式）";
            }
            else
            {
                string fileServer = System.Configuration.ConfigurationManager.AppSettings["fileServer"];
                try
                {
                    using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                    {
                        if (unc.NetUseWithCredentials(cfg.UNCPath,
                                                 cfg.User,
                                                 cfg.Domain,
                                                 cfg.Password))
                        {
                            var now = DateTime.Now;
                            var yyyyMM = now.ToString("yyyyMM");
                            var yyyyMMdd = now.ToString("yyyyMMdd");
                            var dir = cfg.UNCPath + "\\" + yyyyMM + "\\" + yyyyMMdd + "\\";
                            var filename = Utils.SHA1Stream(file.InputStream) + ".jpg";

                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }

                            file.SaveAs(dir + filename);
                            model.FileName = Path.GetFileName(file.FileName);
                            model.FileUrl = fileServer + yyyyMM + "/" + yyyyMMdd + "/" + filename;
                            model.ReturnFlag = true;
                            model.Message = "上传成功。";
                        }
                        else
                        {
                            model.Message = "上传失败，错误代码：" + unc.LastError;
                        }
                    }
                }
                catch (Exception ex)
                {
                    model.Message = ex.Message;
                }
            }
            return model;
        }

        /// <summary>
        /// 远程请求数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static T RemoteRequest<T>(Uri requestUri)
            where T : class,new()
        {
            T result = new T();
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                    string jsonResult = sr.ReadToEnd();
                    result = JsonConvert.DeserializeObject(jsonResult, typeof(T)) as T;
                }
                else 
                {
                    ChatLog.GetInstance().FormatMessage("Error01:远程请求数据错误，方法:chathelper:RemoteRequest，来源URL:{0},response状态{1}", requestUri, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return result;
        }


    }
}
