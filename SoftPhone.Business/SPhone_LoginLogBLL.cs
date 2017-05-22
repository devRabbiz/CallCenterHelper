using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using Tele.DataLibrary;
using SoftPhone.Entity;
using System.Data.Entity.SqlServer;

namespace SoftPhone.Business
{
    public class SPhone_LoginLogBLL
    {
        #region 登录
        public static bool Login(string EmployeeID)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                var info = db.SPhone_LoginLog.Where(x => x.EmployeeID == EmployeeID &&
                   SqlFunctions.DateDiff("d", x.LoginDate, DateTime.Now) == 0).FirstOrDefault();
                if (info == null)
                {
                    info = new SPhone_LoginLog()
                    {
                        ID = Guid.NewGuid(),
                        EmployeeID = EmployeeID,
                        FirstloginTime = DateTime.Now,
                        LoginDate = DateTime.Now,
                        CreateTime = DateTime.Now,
                        CreateBy = EmployeeID
                    };
                    db.SPhone_LoginLog.Add(info);
                }
                else
                {
                    info.LoginDate = DateTime.Now;
                    info.UpdateBy = EmployeeID;
                    info.UpdateTime = DateTime.Now;
                }
                db.SaveChanges();
            }
            return true;
        }
        #endregion

        #region 登出
        public static bool Logout(string EmployeeID)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                var info = db.SPhone_LoginLog.Where(x => x.EmployeeID == EmployeeID &&
                   x.LoginDate < DateTime.Now).OrderByDescending(x => x.CreateTime).FirstOrDefault();
                if (info != null)
                {
                    if (info.FirstlogoutTime == null)
                    {
                        info.FirstlogoutTime = DateTime.Now;
                    }
                    info.Lastlogout = DateTime.Now;
                    info.UpdateBy = EmployeeID;
                    info.UpdateTime = DateTime.Now;
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
