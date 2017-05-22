using SoftPhone.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tele.Management.Pages.Voice.TFNDNIS
{
    public partial class TFNQView : PageBaseV2
    {
        private long TFN;

        protected void Page_Load(object sender, EventArgs e)
        {
            TFN = long.Parse(Request.QueryString["TFN"] ?? "0");

            if (!IsPostBack)
            {
                if (TFN > 0)
                {
                    txtQ.DataValueField = "dbid";
                    txtQ.DataTextField = "name";
                    txtQ.DataSource = GenesysBLL.Proc_GetCfgSkill().ToList();
                    txtQ.DataBind();

                    var currentQS = dc.SPhone_TFNQueueRel.Where(x => x.TFNID == TFN).Select(x => x.QueueID).ToList();

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
                    var info = new SoftPhone.Entity.SPhone_TFNQueueRel()
                    {
                        TFNID = TFN,
                        QueueID = long.Parse(item.Value),
                        QueueName = item.Text,
                        CreateBy = base.UserInfo.LogonID,
                        CreateTime = DateTime.Now
                    };
                    if (dc.SPhone_TFNQueueRel.Count(x => x.QueueID == info.QueueID && x.TFNID == info.TFNID) == 0)
                    {
                        dc.SPhone_TFNQueueRel.Add(info);
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