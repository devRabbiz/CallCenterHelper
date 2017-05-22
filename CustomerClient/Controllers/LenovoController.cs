using Chat.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerClient.Controllers
{
    public class LenovoController : Controller
    {
        //
        // GET: /Lenovo/

        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult Trace(string message)
        {
            ChatLog.GetInstance().FormatMessage("LenovoTrace|" + Server.UrlDecode(message));
            return Json("True", JsonRequestBehavior.AllowGet);
        }

    }
}
