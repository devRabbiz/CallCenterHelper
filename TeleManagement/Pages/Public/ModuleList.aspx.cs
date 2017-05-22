
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;
using SoftPhone.Entity;
using System.Linq.Expressions;

namespace Tele.Management.Pages
{
    public partial class ModuleList : PageBase<SPhone_Module>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                BindParentModule();
            }
            BindRepeater();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater();
        }
        protected void pager1_OnPageChanged(object sender, EventArgs e)
        {
            BindRepeater();
        }
        protected void rptList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Guid id = Guid.Empty;
            Guid.TryParse((string)e.CommandArgument, out id);
            if (e.CommandName == "Status")
            {
                using (var db = DCHelper.SPhoneContext())
                {
                    SPhone_Module dn = db.SPhone_Module.Where(item => item.ModuleID == id).FirstOrDefault();
                    if (dn != null)
                    {
                        db.SPhone_Module.Remove(dn);
                        db.SaveChanges();

                        BindRepeater();
                    }
                }
            }
        }
        #endregion

        #region Properties

        private List<SPhone_Module> modules = null;
        protected List<SPhone_Module> Modules
        {
            get
            {
                if (modules == null)
                {
                    using (var db = DCHelper.SPhoneContext())
                    {
                        modules = db.SPhone_Module.ToList();
                    }
                }
                return modules;
            }
        }

        #endregion

        #region Methods

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            if (!string.IsNullOrEmpty(this.ddlParentModule.SelectedValue))
                context["ParentModuleID"] = this.ddlParentModule.SelectedValue;
            context["ModuleName"] = this.txtModuleName.Text.Trim();
            context["ModuleUrl"] = this.txtModuleUrl.Text.Trim();
        }

        protected string GetModuleNameByID(string typeID)
        {
            Guid id = new Guid(typeID);
            SPhone_Module type = this.Modules.Find(item => item.ModuleID == id);
            return (type != null) ? type.ModuleName : string.Empty;
        }

        #endregion

        #region BindRepeater

        private void BindRepeater()
        {
            var searcher = GetSearcher();
            using (var db = DCHelper.SPhoneContext())
            {
                var query = db.SPhone_Module.Where(item => 1 == 1);
                if (searcher.ParentModuleID != Guid.Empty)
                    query = query.Where(item => item.ParentModuleID == searcher.ParentModuleID);
                if (!string.IsNullOrEmpty(searcher.ModuleName))
                    query = query.Where(item => item.ModuleName.IndexOf(searcher.ModuleName) != -1);
                if (!string.IsNullOrEmpty(searcher.ModuleUrl))
                    query = query.Where(item => item.ModuleUrl.IndexOf(searcher.ModuleUrl) != -1);
                this.pager1.RecordCount = query.Count();
                this.rptList.DataSource = query.OrderBy(item => item.CreateTime)
                    .Skip((this.pager1.CurrentPageIndex - 1) * this.pager1.PageSize).Take(this.pager1.PageSize).ToList();
                this.rptList.DataBind();
            }
        }


        private void BindParentModule()
        {
            this.ddlParentModule.DataSource = this.Modules.Where(item => item.IsLeaf == 0);
            this.ddlParentModule.DataTextField = "ModuleName";
            this.ddlParentModule.DataValueField = "ModuleID";
            this.ddlParentModule.DataBind();
            this.ddlParentModule.Items.Insert(0, new ListItem("——全部模块——", ""));
        }

        #endregion

    }
}