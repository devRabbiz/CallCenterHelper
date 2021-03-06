﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace Tele.Common
{
    public static class ChatHelper
    {

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
        /// 写文件
        /// </summary>
        public static void WriteFile(string path, string newText)
        {
            CheckPath(path);
            if (File.Exists(path))
                File.SetAttributes(path, FileAttributes.Normal);
            FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream, Encoding.Default);
            try
            {
                writer.Write(newText);
            }
            finally
            {
                writer.Close();
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



            if (System.Web.HttpContext.Current.Request.Headers["ocx"] == null && !enabledTypes.Exists(type => type == file.ContentType))
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
                    }
                }
                catch (Exception ex)
                {
                    model.Message = ex.Message;
                }
            }
            return model;
        }


    }
}
