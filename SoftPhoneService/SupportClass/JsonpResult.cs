using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tele.Common.Helper;

namespace SoftPhoneService.SupportClass
{
    /// <summary>
    /// jsonp输出,支持jsoncallback、callback
    /// </summary>
    public class JsonpResult : JsonResult
    {
        public JsonpResult(object Data, JsonRequestBehavior JsonRequestBehavior = JsonRequestBehavior.DenyGet)
        {
            this.Data = Data;
            this.JsonRequestBehavior = JsonRequestBehavior;
        }

        #region ExecuteResult
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("JsonRequest_GetNotAllowed");
            }
            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }
            if (this.Data != null)
            {
                if (context.HttpContext.Request.Params["jsoncallback"] != null)
                {
                    string callback = context.HttpContext.Request.Params["jsoncallback"];
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    response.Write(string.Format("{0}({1})", callback, serializer.Serialize(this.Data)));
                }
                else if (context.HttpContext.Request.Params["callback"] != null)
                {
                    string callback = context.HttpContext.Request.Params["callback"];
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    response.Write(string.Format("{0}({1})", callback, serializer.Serialize(this.Data)));
                }
                else
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    response.Write(serializer.Serialize(this.Data));
                }
            }
        }
        #endregion
    }
}