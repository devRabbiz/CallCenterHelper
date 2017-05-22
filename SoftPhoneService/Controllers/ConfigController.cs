using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftPhoneService.Controllers
{
    public class ConfigController : Controller
    {
        //
        // GET: /Config/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 验证用户名密码是否正确
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userPassword">密码</param>
        /// <returns>Json(Models.LoginResult)</returns>
        public ActionResult Login(string userName, string userPassword)
        {
            var result = SupportClass.CfgServerHelper.Authenticate(userName, userPassword);
            return Json(result);
        }

        /// <summary>
        /// 获取座席能力
        /// </summary>
        /// <returns>Json(Models.MediaCapacity)</returns>
        public ActionResult GetEmployeeMediaCapacity(string employeeId)
        {
            var result = SupportClass.CfgServerHelper.GetEmployeeMediaCapacity(employeeId);
            return Json(result);
        }


    }
}
