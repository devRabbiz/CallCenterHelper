using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Tele.Common;

namespace Tele.Management.Pages
{
    public partial class FileDownload : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            string fileName = Request.QueryString["fileID"];
            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = string.Format("{0}Documents\\TempFolder\\{1}"
                    , this.Request.PhysicalApplicationPath, HttpUtility.UrlDecode(fileName));
                AsposeHelper.DownloadFile(fileName);
            }
        }

    }
}

