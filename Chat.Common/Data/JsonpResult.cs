using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Chat.Common
{
    public class JsonpResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType)) this.ContentType = "application/json";
            response.ContentType = this.ContentType;
            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data != null)
            {
                //跨域调用需要修改json格式jsoncallback
                if (context.HttpContext.Request.Params.AllKeys.Contains("jsoncallback"))
                {
                    string callback = context.HttpContext.Request.Params["jsoncallback"];
                    response.Write(callback + "(" + JsonConvert.SerializeObject(this.Data) + ")");
                }
                else
                {
                    response.Write(JsonConvert.SerializeObject(this.Data));
                }
            }

        }

    }
}
