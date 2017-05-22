using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftPhoneService.Controllers
{
    public class ScreenController : Controller
    {
        //
        // GET: /Screen/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Models.LoginVM vm)
        {
            var appUserName = System.Web.Configuration.WebConfigurationManager.AppSettings["Screen.UserName"];
            var appPWD = System.Web.Configuration.WebConfigurationManager.AppSettings["Screen.PWD"];
            if (vm.UserName == appUserName && vm.PWD == appPWD)
            {
                HttpCookie hc = new HttpCookie("screen");
                hc.Value = "Y";
                Response.AppendCookie(hc);
                return RedirectToAction("Show");
            }

            return View();
        }

        public ActionResult Show()
        {
            var hc = Request.Cookies["screen"];
            if (hc != null && hc.Value == "Y")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

    }
}
