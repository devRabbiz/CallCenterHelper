using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Business
{
    public class Sphone_QProcessingRest
    {
        /// <summary>
        /// 设置中的队列工作时间
        /// </summary>
        /// <returns></returns>
        public static List<SoftPhone.Entity.Proc_GetProcessingQList_Result> GetProcessingQList()
        {
            using (var db = Tele.DataLibrary.DCHelper.SPhoneContext())
            {
                return db.Proc_GetProcessingQList().ToList();
            }
        }

        /// <summary>
        /// 今日休息队列
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRestQ()
        {
            using (var db = Tele.DataLibrary.DCHelper.SPhoneContext())
            {
                return db.Proc_GetTodayRestQList().ToList();
            }
        }
    }
}
