using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.Commons.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
namespace SoftPhoneService
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(MvcApplication));
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 参数默认值
            );
        }

        private static bool AutoStart_cfg;
        private static bool AutoStart_stat;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            #region genesys
            AutoStart_cfg = System.Web.Configuration.WebConfigurationManager.AppSettings["AutoStart.cfg"] == "1";
            AutoStart_stat = System.Web.Configuration.WebConfigurationManager.AppSettings["AutoStart.stat"] == "1";
            log.Info("Application_Start");

            if (AutoStart_cfg)
            {
                SupportClass.CfgServerHelper.Start();
            }
            Thread.Sleep(1000);
            if (AutoStart_stat)
            {
                SupportClass.StatServerHelper.Start();
            }
            #endregion
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            log.Error("应用程序发生错误", ex);
        }

        protected void Application_End()
        {
            #region genesys
            log.Info("Application_End");
            if (AutoStart_cfg)
            {
                SupportClass.CfgServerHelper.End();
            }
            Thread.Sleep(1000);
            if (AutoStart_stat)
            {
                SupportClass.StatServerHelper.End();
            }
            #endregion
        }
    }
}