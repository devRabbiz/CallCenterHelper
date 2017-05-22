
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
    public partial class BlackListList : PageBase<SPhone_BlackList>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                BindBlackListType();

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
                    SPhone_BlackList dn = db.SPhone_BlackList.Where(item => item.BlackListID == id).FirstOrDefault();
                    if (dn != null)
                    {
                        dn.IsDeleted = 1;
                        db.SaveChanges();

                        BindRepeater();
                    }
                }
            }
        }
        #endregion

        #region Properties

        private List<SPhone_BlackListType> types = null;
        protected List<SPhone_BlackListType> Types
        {
            get
            {
                if (types == null)
                {
                    using (var db = DCHelper.SPhoneContext())
                    {
                        types = db.SPhone_BlackListType.ToList();
                    }
                }
                return types;
            }
        }

        #endregion

        #region Protected Methods

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            if (!string.IsNullOrEmpty(this.ddlBlackListType.SelectedValue))
                context["BlackListTypeID"] = this.ddlBlackListType.SelectedValue;
            context["BillNo"] = this.txtBillNo.Text.Trim();
            context["IsDeleted"] = 0;
        }

        protected string GetTypeNameByID(string typeID)
        {
            Guid id = new Guid(typeID);
            SPhone_BlackListType type = this.Types.Find(item => item.BlackListTypeID == id);
            return (type != null) ? type.TypeName : string.Empty;
        }

        #endregion

        #region BindRepeater

        private void BindBlackListType()
        {
            this.ddlBlackListType.DataSource = this.Types;
            this.ddlBlackListType.DataTextField = "TypeName";
            this.ddlBlackListType.DataValueField = "BlackListTypeID";
            this.ddlBlackListType.DataBind();
            this.ddlBlackListType.Items.Insert(0, new ListItem("——不限——", ""));
        }

        protected void BindRepeater()
        {
            var searcher = GetSearcher();
            using (var db = DCHelper.SPhoneContext())
            {
                var query = db.SPhone_BlackList.Where(item => item.IsDeleted == 0);
                if (searcher.BlackListTypeID.HasValue)
                    query = query.Where(item => item.BlackListTypeID == searcher.BlackListTypeID);
                if (!string.IsNullOrEmpty(searcher.BillNo))
                    query = query.Where(item => item.BillNo.IndexOf(searcher.BillNo) != -1);

                this.pager1.RecordCount = query.Count();
                this.rptList.DataSource = query.OrderByDescending(item => item.CreateTime)
                    .Skip((this.pager1.CurrentPageIndex - 1) * this.pager1.PageSize).Take(this.pager1.PageSize).ToList();
                this.rptList.DataBind();
            }
        }


        #endregion

    }
}