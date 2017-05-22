using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftPhoneToolBar.Controllers
{
    public class SoftPhoneToolBarController : BaseController
    {
        //首页
        public ActionResult Index()
        {
            Models.SoftPhoneToolBarVM vm = new Models.SoftPhoneToolBarVM();

            var hc = Request.Cookies["softphone"];
            if (hc != null)
            {
                vm.Place = hc["place"];
                vm.DN = hc["dn"];
                vm.employee_id = hc["employee_id"];
                vm.IsEmergency = int.Parse(hc["isemergency"]);
                vm.password = hc["password"];
            }

            return View(vm);
        }

        public ActionResult CCForBusiness()
        {
            Models.SoftPhoneToolBarVM vm = new Models.SoftPhoneToolBarVM();

            var hc = Request.Cookies["softphone"];
            if (hc != null)
            {
                vm.Place = hc["place"];
                vm.DN = hc["dn"];
                vm.employee_id = hc["employee_id"];
                vm.IsEmergency = int.Parse(hc["isemergency"]);
                vm.password = hc["password"];
            }

            return View(vm);
        }

        public ActionResult CCForBusinessESD()
        {
            Models.SoftPhoneToolBarVM vm = new Models.SoftPhoneToolBarVM();

            var hc = Request.Cookies["softphoneESD"];
            if (hc != null)
            {
                vm.Place = hc["place"];
                vm.DN = hc["dn"];
                vm.employee_id = hc["employee_id"];
                vm.IsEmergency = int.Parse(hc["isemergency"]);
                vm.password = hc["password"];
            }

            return View(vm);
        }

        //转接
        public ActionResult TransferSearch()
        {
            return View();
        }

        //重试
        public ActionResult ReTry()
        {
            return View();
        }
    }
}
