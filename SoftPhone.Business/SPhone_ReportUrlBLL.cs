using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tele.DataLibrary;

namespace SoftPhone.Business
{
    public class SPhone_ReportUrlBLL
    {
        public static List<SPhone_ReportUrl> GetQuery()
        {
            using (var db = DCHelper.SPhoneContext())
            {
                return db.SPhone_ReportUrl.AsNoTracking().OrderBy(x => x.ReportName).ToList();
            }
        }

        public static SPhone_ReportUrl Find(Guid GUID)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                return db.SPhone_ReportUrl.FirstOrDefault(x => x.GUID == GUID);
            }
        }
    }
}
