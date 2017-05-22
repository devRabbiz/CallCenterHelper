using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftPhoneToolBar.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index(string employee_id, string source = "SDI")
        {
            if (string.IsNullOrEmpty(employee_id))
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Models.LoginVM vm = new Models.LoginVM();
                vm.employee_id = employee_id;
                vm.DN = "";
                vm.password = "";
                vm.IsEmergency = 0;
                if (source == "SDI")
                {
                    HttpCookie hc = new HttpCookie("softphone");
                    hc.Values["employee_id"] = vm.employee_id;
                    hc.Values["dn"] = vm.DN;
                    hc.Values["place"] = vm.Place;
                    hc.Values["password"] = vm.password;
                    hc.Values["isemergency"] = vm.IsEmergency.ToString();
                    hc.Expires = DateTime.Now.AddDays(+365);
                    Response.AppendCookie(hc);
                    return RedirectToAction("CCForBusiness", "SoftPhoneToolBar");
                }
                else if (source == "ESD")
                {
                    HttpCookie hc = new HttpCookie("softphoneESD");
                    hc.Values["employee_id"] = vm.employee_id;
                    hc.Values["dn"] = vm.DN;
                    hc.Values["place"] = vm.Place;
                    hc.Values["password"] = vm.password;
                    hc.Values["isemergency"] = vm.IsEmergency.ToString();
                    hc.Expires = DateTime.Now.AddDays(+365);
                    Response.AppendCookie(hc);
                    return RedirectToAction("CCForBusinessESD", "SoftPhoneToolBar");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
        }

        public ActionResult Login()
        {
            Models.LoginVM vm = new Models.LoginVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Login(Models.LoginVM vm)
        {
            HttpCookie hc = new HttpCookie("softphone");
            hc.Values["employee_id"] = vm.employee_id;
            hc.Values["dn"] = vm.DN;
            hc.Values["place"] = vm.Place;
            hc.Values["password"] = vm.password;
            hc.Values["isemergency"] = vm.IsEmergency.ToString();
            hc.Expires = DateTime.Now.AddDays(+365);
            Response.AppendCookie(hc);

            return RedirectToAction("Index", "SoftPhoneToolBar");
        }
    }
}
