using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Tele.Common
{
    public class BatchDownloadResponse
    {
        #region Properties

        /// <summary>
        /// 下载的文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 错误信息集合
        /// Key：行数，Value：错误提示
        /// </summary>
        private Dictionary<int, string> Errors { get; set; }

        #endregion

        #region Constructors

        public BatchDownloadResponse(string fileName)
        {
            this.FileName = fileName;
            this.Errors = new Dictionary<int, string>();
        }

        #endregion

        #region Methods

        public void AddError(int rowID, string message)
        {
            string msg = message;
            if (this.Errors.ContainsKey(rowID))
                msg = string.Format("{0}|{1}", this.Errors[rowID], message);
            this.Errors[rowID] = msg;
        }

        public List<string> GetErrors()
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<int, string> item in this.Errors)
                result.Add(string.Format("异常：第 {0} 行。错误信息：{1}", item.Key, item.Value));
            return result;
        }

        #endregion
    }


    public class BatchUploadResponse
    {
        #region Properties

        /// <summary>
        /// 上传的文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 是否已通过校验
        /// </summary>
        public bool IsVerified
        {
            get
            {
                return this.Errors == null || this.Errors.Count == 0;
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        public DataTable Data { get; set; }

        /// <summary>
        /// 错误信息集合
        /// Key：行数，Value：错误提示
        /// </summary>
        private Dictionary<int, string> Errors { get; set; }

        /// <summary>
        /// 警告信息集合
        /// Key：行数，Value：错误提示
        /// </summary>
        private Dictionary<int, string> Warnings { get; set; }

        #endregion

        #region Constructors

        public BatchUploadResponse(string fileName)
        {
            this.FileName = fileName;
            this.Errors = new Dictionary<int, string>();
            this.Warnings = new Dictionary<int, string>();
        }

        #endregion

        #region Methods

        public void AddError(int rowID, string message)
        {
            string msg = message;
            if (this.Errors.ContainsKey(rowID))
                msg = string.Format("{0}|{1}", this.Errors[rowID], message);
            this.Errors[rowID] = msg;
        }

        public void AddWarning(int rowID, string message)
        {
            string msg = message;
            if (this.Warnings.ContainsKey(rowID))
                msg = string.Format("{0}|{1}", this.Errors[rowID], message);
            this.Warnings[rowID] = msg;
        }

        public List<string> GetErrors()
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<int, string> item in this.Errors)
                result.Add(string.Format("异常：第 {0} 行。错误信息：{1}", item.Key, item.Value));
            return result;
        }

        public List<string> GetWarnings()
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<int, string> item in this.Warnings)
                result.Add(string.Format("警告：第 {0} 行。提示信息：{1}", item.Key, item.Value));
            return result;
        }

        #endregion
    }

}
