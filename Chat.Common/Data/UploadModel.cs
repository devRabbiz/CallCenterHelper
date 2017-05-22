using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chat.Common
{
    public class UploadModel
    {
        /// <summary>
        /// 上传是否成功
        /// </summary>
        public bool ReturnFlag { get; set; }

        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件链接
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }
    }
}