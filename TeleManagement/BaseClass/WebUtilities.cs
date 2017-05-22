using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Tele.Common;

namespace Tele.Management.BaseClass
{
    public static class WebUtilities
    {

        /// <summary>
        /// 后台提示信息到页面上
        /// </summary>
        /// <param name="page"></param>
        /// <param name="alertInfo"></param>
        private static void Alert(this Page page, string alertInfo)
        {
            alertInfo = Utils.CheckAlterMessage(alertInfo);
            string js = string.Format("<script type='text/javascript' defer='defer'>alert('{0}');</script>", alertInfo);
            ScriptManager.RegisterStartupScript(page, page.GetType(), "Alter", js, false);
        }

        /// <summary>
        /// 提示信息后跳转到指定页面
        /// </summary>
        /// <param name="page"></param>
        /// <param name="alertInfo"></param>
        /// <param name="url"></param>
        private static void AlertAndRedirect(this Page page, string alertInfo, string url)
        {
            alertInfo = Utils.CheckAlterMessage(alertInfo);
            string js = string.Empty;
            if (string.IsNullOrEmpty(alertInfo))
                js = string.Format("<script type='text/javascript' defer='defer'>window.location='{0}'</script>", url);
            else
                js = string.Format("<script type='text/javascript' defer='defer'>alert('{0}');window.location='{1}'</script>", alertInfo, url);
            ScriptManager.RegisterStartupScript(page, page.GetType(), "AlertAndRedirect", js, false);
        }
    }
}