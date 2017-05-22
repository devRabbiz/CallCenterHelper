using Microsoft.Reporting.WebForms;
using SoftPhone.Business;
using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tele.Management.Pages.Report
{
    public partial class Detail : PageBase<SPhone_ReportUrl>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string guid = Request.QueryString["guid"] ?? "";

                if (guid != "")
                {
                    var GUID = Guid.Empty;

                    try
                    {
                        GUID = Guid.Parse(guid);
                    }
                    catch { }

                    var info = SPhone_ReportUrlBLL.Find(GUID);
                    if (info != null)
                    {
                        ReportViewerServerCredentials IServer = new ReportViewerServerCredentials();

                        ReportViewer1.ServerReport.ReportServerCredentials = IServer;
                        ReportViewer1.ServerReport.ReportServerUrl = IServer._reportServerUri;
                        ReportViewer1.ProcessingMode = ProcessingMode.Remote;

                        ReportViewer1.ServerReport.ReportPath = info.ReportPath;

                        this.Title = info.ReportName;
                    }
                }
            }
        }
    }

    [Serializable]
    public class ReportViewerServerCredentials : IReportServerCredentials
    {
        public Uri _reportServerUri = null;
        private string _UserName = string.Empty;
        private string _PassWord = string.Empty;
        private string _DomainName = string.Empty;

        public ReportViewerServerCredentials()
        {
            _reportServerUri = new Uri(System.Configuration.ConfigurationManager.AppSettings["Report_ServerUrl"]);
            _UserName = System.Configuration.ConfigurationManager.AppSettings["Report_UserName"];
            _PassWord = System.Configuration.ConfigurationManager.AppSettings["Report_UserPassword"];
            _DomainName = System.Configuration.ConfigurationManager.AppSettings["Report_Domain"];
        }

        public WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {

                if (string.IsNullOrEmpty(_UserName) || string.IsNullOrEmpty(_PassWord))
                {
                    return null;
                }
                else if (string.IsNullOrEmpty(_DomainName))
                {
                    //2.若未指定域，则表示当前请求域
                    return new NetworkCredential(_UserName, _PassWord);
                }
                else
                {
                    //3.用户+域认证
                    return new NetworkCredential(_UserName, _PassWord, _DomainName);
                }
            }
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }
}