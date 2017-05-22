

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;
using SoftPhone.Entity;
using SoftPhone.Business;

namespace Tele.Management.Pages
{
    public partial class RoleView : PageBase<SPhone_Role>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Public/RoleList.aspx";
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
                return "RoleID";
            }
        }

        #endregion

        #region Override Methods

        protected override SPhone_Role LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_Role.FirstOrDefault(item => item.RoleID == entityID);
            }
        }

        protected override void InitView(SPhone_Role model)
        {
            Dictionary<string, int> selectedIDs = new Dictionary<string, int>();
            if (model != null)
            {
                this.txtRoleName.Text = model.RoleName;
                selectedIDs = SPhone_RoleBLL.GetSelectedIDs(model.RoleID);
            }
            this.hdTree.Value = SPhone_RoleBLL.GetTreeValue(selectedIDs);
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["RoleName"] = this.txtRoleName.Text.Trim();
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

        protected override void CustomValidation(SPhone_Role data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.RoleName))
                errorMessages.Add("RoleName 不能为空！");
        }

        protected override void PersistData(SPhone_Role data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.SPhone_Role.Add(data);
                else
                {
                    var entity = db.SPhone_Role.FirstOrDefault(item => item.RoleID == data.RoleID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();


                // 处理关系表
                string selectedIDs = this.hdSelectedOrg.Value;
                List<string> deleteIDs = new List<string>();
                List<string> addIDs = new List<string>();
                Dictionary<string, int> selected = SPhone_RoleBLL.GetSelectedIDs(selectedIDs);
                Dictionary<string, int> owned = SPhone_RoleBLL.GetSelectedIDs(data.RoleID);
                // 构造新增/修改的集合
                if (selected != null && selected.Count > 0)
                    addIDs = selected.Keys.ToList().FindAll(id => !owned.ContainsKey(id));
                // 构造删除的集合
                if (owned != null && owned.Count > 0)
                    deleteIDs = owned.Keys.ToList().FindAll(id => !selected.ContainsKey(id));
                PersistData(data.RoleID, deleteIDs, addIDs);
                BindTreeView(data.RoleID);
            }
        }

        protected void PersistData(Guid roleID, List<string> deleteIDs, List<string> addIDs)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (deleteIDs != null && deleteIDs.Count > 0)
                    deleteIDs.ForEach(id =>
                    {
                        SPhone_RoleModule up = db.SPhone_RoleModule.First(item => item.RoleID == roleID && item.ModuleID == new Guid(id));
                        if (up != null)
                            db.SPhone_RoleModule.Remove(up);
                    });
                if (addIDs != null && addIDs.Count > 0)
                    addIDs.ForEach(id =>
                    {
                        SPhone_RoleModule model = new SPhone_RoleModule();
                        model.RoleID = roleID;
                        model.ModuleID = new Guid(id);
                        model.CreateBy = this.UserInfo.LogonID;
                        model.CreateTime = DateTime.Now;

                        db.SPhone_RoleModule.Add(model);
                    });

                db.SaveChanges();
            }
        }

        #endregion

        #region MyRegion


        protected void BindTreeView(Guid roleID)
        {
            Dictionary<string, int> selectedIDs = new Dictionary<string, int>();
            if (!string.IsNullOrEmpty(this.PrimaryValue))
            {
                selectedIDs = SPhone_RoleBLL.GetSelectedIDs(roleID);
            }
            this.hdTree.Value = SPhone_RoleBLL.GetTreeValue(selectedIDs);
        }


        #endregion
    }
}