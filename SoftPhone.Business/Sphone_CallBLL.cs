using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tele.DataLibrary;

namespace SoftPhone.Business
{
    public class Sphone_CallBLL
    {
        #region 创建Call
        public static void Create(string CallID, string EmployeeID, string ConnectionID, string ANI, string DNIS, int InOut, string CurrentQueueName, string FromQueueName, string PlaceIP)
        {
            var info = new Sphone_Call();
            info.CallID = CallID;
            info.EmployeeID = EmployeeID;
            info.ConnectionID = ConnectionID;
            info.ANI = ANI;
            info.DNIS = DNIS;
            info.InOut = InOut;
            info.CurrentQueueName = CurrentQueueName;
            info.FromQueueName = FromQueueName;

            info.CallBeginTime = DateTime.Now;
            info.CreateBy = EmployeeID;
            info.CreateTime = DateTime.Now;
            info.PlaceIP = PlaceIP;
            using (var db = DCHelper.SPhoneContext())
            {
                db.Sphone_Call.Add(info);
                db.SaveChanges();
            }
        }
        #endregion

        #region 案面时间
        public static void SetDeskTime(string CallID, string EmployeeID)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                var info = db.Sphone_Call.Find(CallID);
                if (info != null && info.EmployeeID == EmployeeID)
                {
                    info.DeskTime = DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
        #endregion

        #region 挂机
        public static void CallEnd(string CallID, string EmployeeID, string CustomerID, string NextQueueName, int IsConference, int IsTransfer, int IsTransferEPOS)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                var info = db.Sphone_Call.Find(CallID);
                if (info != null && info.EmployeeID == EmployeeID)
                {
                    info.CallEndTime = DateTime.Now;
                    info.UpdateTime = DateTime.Now;
                    info.UpdateBy = EmployeeID;

                    try
                    {
                        if (!string.IsNullOrEmpty(CustomerID))
                        {
                            info.CustomerID = CustomerID;
                        }
                    }
                    catch { }
                    info.NextQueueName = NextQueueName;
                    info.IsConference = IsConference;
                    info.IsTransfer = IsTransfer;
                    info.IsTransferEPOS = IsTransferEPOS;

                    db.SaveChanges();
                }
            }
        }
        #endregion
    }
}
