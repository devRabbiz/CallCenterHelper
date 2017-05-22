using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Common
{
    [Serializable]
    public class AjaxReturn
    {
        public void SetError(string Message, int Code = -1)
        {
            this.Code = Code;
            this.Message = Message;
        }

        /// <summary>
        /// 负数则说明有错误
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息提示
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public dynamic d { get; set; }
    }
}
