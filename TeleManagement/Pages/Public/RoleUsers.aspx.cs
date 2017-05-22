
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;
using SoftPhone.Entity;
using System.Linq.Expressions;
using SoftPhone.Business;

namespace Tele.Management.Pages
{
    public partial class RoleUsers : PageBase<SPhone_UserPermission>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Public/RoleList.aspx";
            if (!this.IsPostBack)
            {
                BindRole();
                // BindTreeView();
                BindUsers();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //BindTreeView();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            PersistData();
        }

        #endregion

        #region Properties

        protected override string PrimaryKey
        {
            get
            {
                return "RoleID";
            }
        }

        #endregion

        #region Override Methods

        protected override SPhone_UserPermission LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_UserPermission.FirstOrDefault(item => item.RoleID == entityID);
            }
        }

        protected void PersistData()
        {
            using (var db = DCHelper.SPhoneContext())
            {
                var roleid = Guid.Parse(ddlRole.SelectedValue);
                db.SPhone_UserPermission.Where(x => x.RoleID == roleid).ToList()
                    .ForEach(x => db.SPhone_UserPermission.Remove(x));

                foreach (ListItem item in listUsers.Items)
                {
                    if (item.Selected)
                    {
                        SPhone_UserPermission model = new SPhone_UserPermission();
                        model.RoleID = new Guid(this.ddlRole.SelectedValue);
                        model.EmployeeID = item.Value;
                        model.CreateBy = this.UserInfo.LogonID;
                        model.CreateTime = DateTime.Now;

                        db.SPhone_UserPermission.Add(model);
                    }
                }



                db.SaveChanges();
            }
        }

        #endregion

        #region BindData

        private void BindRole()
        {
            List<SPhone_Role> lst = new List<SPhone_Role>();
            using (var db = DCHelper.SPhoneContext())
            {
                lst = db.SPhone_Role.ToList();
            }
            this.ddlRole.DataSource = lst;
            this.ddlRole.DataTextField = "RoleName";
            this.ddlRole.DataValueField = "RoleID";
            this.ddlRole.DataBind();
        }

        protected void BindTreeView()
        {
            Dictionary<string, int> selectedIDs = new Dictionary<string, int>();
            string employeeID = this.txtUserID.Text.Trim();
            if (!string.IsNullOrEmpty(this.PrimaryValue))
            {
                this.ddlRole.SelectedValue = this.PrimaryValue;
                selectedIDs = SPhone_UserPermissionBLL.GetSelectedIDs(new Guid(this.PrimaryValue));
            }
            this.hdTree.Value = SPhone_UserPermissionBLL.GetTreeValue(employeeID, selectedIDs);
        }

        protected void BindUsers()
        {
            Dictionary<string, int> selectedIDs = new Dictionary<string, int>();
            string employeeID = this.txtUserID.Text.Trim();
            if (!string.IsNullOrEmpty(this.PrimaryValue))
            {
                ddlRole.Enabled = false;
                this.ddlRole.SelectedValue = this.PrimaryValue;
                selectedIDs = SPhone_UserPermissionBLL.GetSelectedIDs(new Guid(this.PrimaryValue));
            }


            listUsers.DataSource = ProcBLL.Proc_GetCfgAdmin().Select(x => x.employee_id).ToList();
            listUsers.DataBind();

            foreach (ListItem item in listUsers.Items)
            {
                if (selectedIDs.ContainsKey(item.Value))
                {
                    item.Selected = true;
                }

            }
        }


        #endregion

    }
}