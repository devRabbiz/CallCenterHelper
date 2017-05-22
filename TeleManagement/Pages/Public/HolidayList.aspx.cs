
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
    public partial class HolidayList : PageBase<SPhone_Holiday>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                BindHolidayRole();
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
                    SPhone_Holiday entity = db.SPhone_Holiday.Where(item => item.HolidayID == id).FirstOrDefault();
                    if (entity != null)
                    {
                        db.SPhone_Holiday.Remove(entity);
                        db.SaveChanges();

                        BindRepeater();
                    }
                }
            }
        }
        #endregion

        #region Properties

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

        #region Protected Methods

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            if (!string.IsNullOrEmpty(this.ddlType.SelectedValue))
                context["TypeID"] = this.ddlType.SelectedValue;
            if (!string.IsNullOrEmpty(this.ddlHolidayRole.SelectedValue))
                context["HolidayRoleID"] = this.ddlHolidayRole.SelectedValue;
            context["QueueName"] = this.txtSkill.Text.Trim();
            context["DNIS"] = this.txtDNIS.Text.Trim();
        }

        protected string GetRoleNameByID(string typeID)
        {
            Guid id = new Guid(typeID);
            SPhone_HolidayRole type = this.Roles.Find(item => item.HolidayRoleID == id);
            return (type != null) ? type.HolidayRoleName : string.Empty;
        }

        #endregion

        #region BindRepeater

        private void BindHolidayRole()
        {
            this.ddlHolidayRole.DataSource = this.Roles;
            this.ddlHolidayRole.DataTextField = "HolidayRoleName";
            this.ddlHolidayRole.DataValueField = "HolidayRoleID";
            this.ddlHolidayRole.DataBind();

            ListItem li = this.ddlHolidayRole.Items.FindByValue("00000000-0000-0000-0000-000000000001");
            this.ddlHolidayRole.Items.Remove(li);
            this.ddlHolidayRole.Items.Insert(0, new ListItem("——不限——", ""));
        }

        protected void BindRepeater()
        {
            var searcher = GetSearcher();
            using (var db = DCHelper.SPhoneContext())
            {
                var notInRole = Guid.Parse("00000000-0000-0000-0000-000000000001");
                var query = db.SPhone_Holiday.Where(x => x.HolidayRoleID != notInRole);
                if (searcher.HolidayRoleID.HasValue)
                {
                    query = query.Where(x => x.HolidayRoleID == searcher.HolidayRoleID);
                }

                if (!string.IsNullOrEmpty(searcher.QueueName))
                {
                    query = query.Where(x => x.QueueName.Contains(searcher.QueueName));
                }

                if (!string.IsNullOrEmpty(searcher.DNIS))
                {
                    query = query.Where(x => x.DNIS.Contains(searcher.DNIS));
                }

                int typeID = 0;
                int.TryParse(this.ddlType.SelectedValue, out typeID);
                if (typeID > 0)
                {
                    query = query.Where(x => x.TypeID == typeID);
                }
                this.pager1.RecordCount = query.Count();
                this.rptList.DataSource = query.OrderByDescending(x => x.CreateTime)
                    .Skip((this.pager1.CurrentPageIndex - 1) * this.pager1.PageSize).Take(this.pager1.PageSize).ToList();
                this.rptList.DataBind();
            }
        }


        #endregion

    }
}