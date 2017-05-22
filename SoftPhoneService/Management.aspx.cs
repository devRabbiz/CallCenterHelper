using SoftPhone.Entity.Model.cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;

namespace SoftPhoneService
{
    public partial class Management : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                list.DataSource = SupportClass.StatServerHelper.ALLStatisticItems.Values.Where(x => x.REQ_ID > 0 || x.RequireOpen).OrderBy(x => x.Statistic.Uri).ThenBy(x => x.REQ_ID).ToList();
                list.DataBind();


                //var ALLPersons = SupportClass.CfgServerHelper.GetPersonsShort(); //1.先初始化人员（短）
                //var ALLSkills = SupportClass.CfgServerHelper.GetSkills();//3.获取所有队列
                //var statisticItems = SupportClass.StatisticXMLHelper.GetStatisticItems(ALLPersons, ALLSkills);//4.根据person和队列格式化订阅item

            }
        }

        //protected void btnStart_Click(object sender, EventArgs e)
        //{
        //    SupportClass.StatServerHelper.Start();
        //}

        //protected void btnStop_Click(object sender, EventArgs e)
        //{
        //    SupportClass.StatServerHelper.End();
        //}

        //protected void btnOpenSkill_Click(object sender, EventArgs e)
        //{
        //    System.Threading.ThreadPool.QueueUserWorkItem(
        //        new WaitCallback(SupportClass.StatServerHelper.OpenQueneStatistic),
        //        null
        //     );
        //}

        //protected void btnCloseSkill_Click(object sender, EventArgs e)
        //{
        //    System.Threading.ThreadPool.QueueUserWorkItem(
        //        new WaitCallback(SupportClass.StatServerHelper.CloseQueneStatistic),
        //        null
        //     );
        //}

        //protected void btnReInit_Click(object sender, EventArgs e)
        //{
        //    System.Threading.ThreadPool.QueueUserWorkItem(
        //        new WaitCallback(SupportClass.StatServerHelper.ReInit),
        //        true
        //     );
        //}

        //protected void btnCloseOne_Click(object sender, EventArgs e)
        //{
        //    var referenceId = int.Parse(txtReferenceId.Text);
        //    SupportClass.StatServerHelper.CloseStatistic(referenceId);
        //}
    }
}