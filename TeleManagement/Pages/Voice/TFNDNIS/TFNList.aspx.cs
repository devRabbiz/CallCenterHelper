using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
namespace Tele.Management.Pages.Voice.TFNDNIS
{
    public partial class TFNList : PageBaseV2
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var qType = dc.SPhone_NameValue.Where(x => x.TypeCode == "TFNBusi" && ((x.IsDel.HasValue && x.IsDel != 1) || x.IsDel == null)).OrderBy(x => x.Sort).ToList();
                txtTFNBusi.DataValueField = "Value";
                txtTFNBusi.DataTextField = "Name";
                txtTFNBusi.DataSource = qType;
                txtTFNBusi.DataBind();

                BindRepeater();
            }
        }

        private void BindOther()
        {
            if (txtTFNID.Value != "")
            {
                var id = long.Parse(txtTFNID.Value);
                var q1 = dc.SPhone_TFNQueueRel.Include(x => x.SPhone_TFN).Where(x => x.TFNID == id).OrderByDescending(x => x.TFNID).Select(x => new { x.TFNID, x.QueueID, x.SPhone_TFN.TFNName, x.QueueName }).ToList();
                gridQ.DataSource = q1;
                gridQ.DataBind();

                var q2 = dc.SPhone_TFNDNISRel.Include(x => x.SPhone_TFN).Include(x => x.SPhone_DNIS).Where(x => x.TFNID == id).Select(x => new { x.TFNID, x.DNISID, x.SPhone_TFN.TFNName, x.SPhone_DNIS.DNISName }).ToList();
                gridDNIS.DataSource = q2;
                gridDNIS.DataBind();
            }
            else
            {
                gridQ.DataSource = null;
                gridQ.DataBind();
                gridDNIS.DataSource = null;
                gridDNIS.DataBind();
            }
        }

        #region 队列关系
        protected void gridQ_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var TFNID = long.Parse(e.CommandArgument.ToString().Split(',')[0]);
            var QueueID = long.Parse(e.CommandArgument.ToString().Split(',')[1]);
            var info = dc.SPhone_TFNQueueRel.FirstOrDefault(x => x.TFNID == TFNID && x.QueueID == QueueID);
            if (info != null)
            {
                dc.SPhone_TFNQueueRel.Remove(info);
                dc.SaveChanges();
                BindOther();
            }
        }
        #endregion

        #region 小号关系
        protected void gridDNIS_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var TFNID = long.Parse(e.CommandArgument.ToString().Split(',')[0]);
            var DNISID = long.Parse(e.CommandArgument.ToString().Split(',')[1]);
            var info = dc.SPhone_TFNDNISRel.FirstOrDefault(x => x.TFNID == TFNID && x.DNISID == DNISID);
            if (info != null)
            {
                dc.SPhone_TFNDNISRel.Remove(info);
                dc.SaveChanges();
                BindOther();
            }
        }
        #endregion

        #region 大号
        private void BindRepeater()
        {
            var query = dc.SPhone_TFN.Where(x => 1 == 1);
            if (txtTFNName.Text != "")
            {
                query = query.Where(x => x.TFNName.Contains(txtTFNName.Text));
            }
            if (txtTFNBusi.Text != "")
            {
                query = query.Where(x => x.TFNBusi == txtTFNBusi.Text);
            }
            query = query.OrderByDescending(x => x.TFNID);

            gridTFN.DataSource = query.Skip((pager1.CurrentPageIndex - 1) * pager1.PageSize).Take(pager1.PageSize).ToList();
            gridTFN.DataBind();
            pager1.RecordCount = query.Count();

            BindOther();
        }
        protected void gridTFN_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                txtTFNID.Value = e.CommandArgument.ToString();

                BindOther();
            }
            else if (e.CommandName == "Del")
            {
                var id = long.Parse(e.CommandArgument.ToString());
                var info = dc.SPhone_TFN.FirstOrDefault(x => x.TFNID == id);
                dc.SPhone_TFN.Remove(info);
                dc.SaveChanges();
                BindRepeater();
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            txtTFNID.Value = "";
            pager1.CurrentPageIndex = 0;
            BindRepeater();
        }
        protected void pager1_PageChanged(object sender, EventArgs e)
        {
            BindRepeater();
        }
        protected string getTFNBusiName(object input)
        {
            if (input != null)
            {
                foreach (ListItem item in txtTFNBusi.Items)
                {
                    if (item.Value == input.ToString())
                    {
                        return item.Text;
                    }
                }
            }
            return string.Empty;

        }
        #endregion

    }
}