using SoftPhoneService.SupportClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftPhoneService.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// callback、jsoncallback
        /// </summary>
        /// <param name="data"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public JsonResult Jsonp(object data, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            return new SupportClass.JsonpResult(data, behavior);
        }
    }
}