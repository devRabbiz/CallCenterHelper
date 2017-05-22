using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;

namespace Tele.Management.Pages.Voice.TFNDNIS
{
    public partial class TFNDNISView : PageBaseV2
    {
        private long TFN;

        protected void Page_Load(object sender, EventArgs e)
        {
            TFN = long.Parse(Request.QueryString["TFN"] ?? "0");

            if (!IsPostBack)
            {
                if (TFN > 0)
                {
                    txtQ.DataValueField = "DNISID";
                    txtQ.DataTextField = "DNISName";
                    txtQ.DataSource = dc.SPhone_DNIS.Select(x => new { x.DNISID, x.DNISName }).ToList();
                    txtQ.DataBind();

                    var currentQS = dc.SPhone_TFNDNISRel.Where(x => x.TFNID == TFN).Select(x => x.DNISID).ToList();

                    foreach (ListItem item in txtQ.Items)
                    {
                        if (currentQS.Contains(long.Parse(item.Value)))
                        {
                            item.Enabled = false;
                        }
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var save = false;
            foreach (ListItem item in txtQ.Items)
            {
                if (item.Selected)
                {
                    var info = new SoftPhone.Entity.SPhone_TFNDNISRel()
                    {
                        TFNID = TFN,
                        DNISID = long.Parse(item.Value),

                        CreateBy = base.UserInfo.LogonID,
                        CreateTime = DateTime.Now
                    };
                    if (dc.SPhone_TFNDNISRel.Count(x => x.DNISID == info.DNISID && x.TFNID == info.TFNID) == 0)
                    {
                        dc.SPhone_TFNDNISRel.Add(info);
                        save = true;
                    }
                }
            }
            if (save)
            {
                dc.SaveChanges();
                panOK.Visible = true;
            }
        }
    }
}