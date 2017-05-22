

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;
using SoftPhone.Entity;

namespace Tele.Management.Pages
{
    public partial class ModuleView : PageBase<SPhone_Module>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Public/ModuleList.aspx";
            if (!this.IsPostBack)
            {
                BindParentModule();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.SubmitData();
        }

        #endregion

        #region Properties

        protected override string PrimaryKey
        {
            get
            {
                return "ModuleID";
            }
        }

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

        #region Override Methods

        protected override SPhone_Module LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_Module.FirstOrDefault(item => item.ModuleID == entityID);
            }
        }

        protected override void InitView(SPhone_Module model)
        {
            if (model != null)
            {
                if (model.ParentModuleID.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    this.ddlParentModule.SelectedValue = model.ParentModuleID.ToString();
                }
                this.cbIsLeaf.Checked = model.IsLeaf == 1;
                this.txtModuleName.Text = model.ModuleName;
                this.txtModuleUrl.Text = model.ModuleUrl;
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["ParentModuleID"] = this.ddlParentModule.SelectedValue;
            context["IsLeaf"] = this.cbIsLeaf.Checked ? 1 : 0;
            context["ModuleName"] = this.txtModuleName.Text.Trim();
            context["ModuleUrl"] = this.txtModuleUrl.Text.Trim();
            if (string.IsNullOrEmpty(this.PrimaryValue))
            {
                context["CreateBy"] = this.UserInfo.LogonID;
                context["CreateTime"] = DateTime.Now;
            }
            else
            {
                context["UpdateBy"] = this.UserInfo.LogonID;
                context["UpdateTime"] = DateTime.Now;
            }
        }

        protected override void CustomValidation(SPhone_Module data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.ModuleName))
                errorMessages.Add("ModuleName 不能为空！");
            if (string.IsNullOrEmpty(data.ModuleUrl))
                errorMessages.Add("ModuleUrl 不能为空！");
        }

        protected override void PersistData(SPhone_Module data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.SPhone_Module.Add(data);
                else
                {
                    var entity = db.SPhone_Module.FirstOrDefault(item => item.ModuleID == data.ModuleID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region BindParentModule


        private void BindParentModule()
        {
            this.ddlParentModule.DataSource = this.Modules.OrderBy(item => item.CreateTime);
            this.ddlParentModule.DataTextField = "ModuleName";
            this.ddlParentModule.DataValueField = "ModuleID";
            this.ddlParentModule.DataBind();
            this.ddlParentModule.Items.Insert(0, new ListItem("——根目录——", ""));
        }
        #endregion
    }
}