

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
    public partial class HolidayView : PageBase<SPhone_Holiday>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Public/HolidayList.aspx";
            if (!this.IsPostBack)
            {
                BindHolidayRole();

                if ((Request.QueryString["id"] ?? "") == "")
                {
                    BindSkills("");
                }
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
                return "HolidayID";
            }
        }

        private List<SPhone_HolidayRole> roles = null;
        protected List<SPhone_HolidayRole> Roles
        {
            get
            {
                if (roles == null)
                {
                    using (var db = DCHelper.SPhoneContext())
                    {
                        roles = db.SPhone_HolidayRole.ToList();
                    }
                }
                return roles;
            }
        }
        #endregion

        #region Override Methods

        protected override SPhone_Holiday LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_Holiday.FirstOrDefault(item => item.HolidayID == entityID);
            }
        }

        protected override void InitView(SPhone_Holiday model)
        {
            if (model != null)
            {
                this.ddlType.SelectedValue = model.TypeID.ToString();
                this.ddlHolidayRole.SelectedValue = model.HolidayRoleID.ToString();
                this.txtDNIS.Text = model.DNIS;
                this.BindSkills(model.QueueName.ToString());
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["HolidayRoleID"] = this.ddlHolidayRole.SelectedValue;
            context["QueueName"] = this.hdQueueName.Value;
            context["DNIS"] = this.txtDNIS.Text.Trim();
            context["TypeID"] = this.ddlType.SelectedValue;
            context["TypeName"] = this.ddlType.Text;
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

        protected override void CustomValidation(SPhone_Holiday data, List<string> errorMessages)
        {
            //if (string.IsNullOrEmpty(data.QueueName))
            //    errorMessages.Add("QueueName 不能为空！");
        }

        protected override void PersistData(SPhone_Holiday data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                {
                    var isAdd = false;
                    if (txtDNIS.Text.Trim() == "")
                    {
                        foreach (ListItem item in listSkills.Items)
                        {
                            if (item.Selected)
                            {
                                data.QueueName = item.Text;
                                var entity = new SPhone_Holiday();
                                Common.Utils.UpdateEntity<SPhone_Holiday>(entity, data);
                                entity.HolidayID = Guid.NewGuid();
                                db.SPhone_Holiday.Add(entity);
                                isAdd = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (var dnis in txtDNIS.Text.Trim().Split(','))
                        {
                            data.DNIS = dnis;
                            foreach (ListItem item in listSkills.Items)
                            {
                                if (item.Selected)
                                {
                                    data.QueueName = item.Text;

                                    var entity = new SPhone_Holiday();
                                    Common.Utils.UpdateEntity<SPhone_Holiday>(entity, data);
                                    entity.HolidayID = Guid.NewGuid();
                                    db.SPhone_Holiday.Add(entity);

                                    isAdd = true;
                                }
                            }

                            if (!isAdd)
                            {
                                var entity = new SPhone_Holiday();
                                Common.Utils.UpdateEntity<SPhone_Holiday>(entity, data);
                                entity.HolidayID = Guid.NewGuid();
                                db.SPhone_Holiday.Add(entity);

                                isAdd = true;
                            }
                        }
                    }
                    if (!isAdd)
                    {
                        db.SPhone_Holiday.Add(data);
                        isAdd = true;
                    }
                }
                else
                {
                    var entity = db.SPhone_Holiday.FirstOrDefault(item => item.HolidayID == data.HolidayID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region DataBind


        private void BindHolidayRole()
        {
            this.ddlHolidayRole.DataSource = this.Roles;
            this.ddlHolidayRole.DataTextField = "HolidayRoleName";
            this.ddlHolidayRole.DataValueField = "HolidayRoleID";
            this.ddlHolidayRole.DataBind();

            ListItem li = this.ddlHolidayRole.Items.FindByValue("00000000-0000-0000-0000-000000000001");
            this.ddlHolidayRole.Items.Remove(li);
        }

        private void BindSkills(string skillName)
        {
            var query = GenesysBLL.Proc_GetCfgSkill().ToList();
            this.listSkills.DataSource = query;
            this.listSkills.DataBind();

            if (!string.IsNullOrEmpty(skillName))
            {
                listSkills.Items.Insert(0, new ListItem() { Value = "", Text = "--请选择--" });
                listSkills.SelectionMode = ListSelectionMode.Single;
                foreach (ListItem item in listSkills.Items)
                {
                    if (item.Text == skillName)
                    {
                        item.Selected = true;
                    }
                }
            }
        }



        #endregion

    }
}