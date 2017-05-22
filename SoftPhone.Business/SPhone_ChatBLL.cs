using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftPhone.Entity;
using Tele.DataLibrary;

namespace SoftPhone.Business
{
    public class SPhone_ChatBLL
    {

        #region SPhone_Chat

        public static void AddNewChat(SPhone_Chat entity)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                db.SPhone_Chat.Add(entity);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 获取单条聊天数据
        /// </summary>
        public static SPhone_Chat GetChat(string chatID)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                return db.SPhone_Chat.Where(item =>
                    item.ChatID == chatID)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取坐席某时间段的聊天ID列表
        /// </summary>
        public static List<string> GetChatIDList(string employeeID, DateTime beginDate, DateTime endDate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                return db.SPhone_Chat.Where(item =>
                    item.EmployeeID == employeeID
                    && item.ChatBeginTime >= beginDate
                    && item.ChatEndTime <= endDate)
                    .Select(item => item.ChatID).ToList();
            }
        }

        /// <summary>
        /// 获取坐席某时间段的聊天数据
        /// </summary>
        public static List<SPhone_Chat> GetChatList(string employeeID, DateTime beginDate, DateTime endDate, string machineNO = "", string customerID = "")
        {
            using (var db = DCHelper.SPhoneContext())
            {
                var query = db.SPhone_Chat.Where(item => item.ChatBeginTime >= beginDate
                    && item.ChatEndTime <= endDate);
                if (string.IsNullOrEmpty(machineNO) && string.IsNullOrEmpty(customerID))
                {
                    query = query.Where(x => x.EmployeeID == employeeID);
                }

                if (!string.IsNullOrEmpty(machineNO))
                {
                    query = query.Where(x => x.MachineNo.Equals(machineNO));
                }

                if (!string.IsNullOrEmpty(customerID))
                {
                    query = query.Where(x => x.CustomerID.Equals(customerID));
                }

                return query.ToList();
            }
        }



        #endregion

        #region SPhone_ChatText

        public static void AddNewChatText(SPhone_ChatText entity)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                db.SPhone_ChatText.Add(entity);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 根据条件获取术语集合
        /// </summary>
        /// <param name="match"></param>
        public static List<string> GetChatTextList(string queueName, string typeName)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                return db.SPhone_ChatText.Where(item =>
                               item.QueueName.Equals(queueName, StringComparison.CurrentCultureIgnoreCase)
                            && item.SPhone_ChatTextType.ChatTextTypeName.Equals(typeName)
                    ).OrderBy(item => item.SortID).Select(item => item.ChatContent).ToList();
            }
        }

        /// <summary>
        /// chat黑名单
        /// </summary>
        /// <param name="CustName"></param>
        /// <returns></returns>
        public static int IsChatInBlack(string CustName)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                var typeGUID = Guid.Parse("03679a4d-2408-42b6-aa83-4e05885fea9d");

                var query = db.SPhone_BlackList.Count(x =>
                    x.BlackListTypeID == typeGUID &&
                    x.IsDeleted == 0 &&
                    x.BillNo == CustName
                    );
                if (query > 0)
                {
                    return 1;
                }
                return 0;
            }
        }
        #endregion

        public static void Update<T>(T entity)
            where T : class
        {
            using (var db = DCHelper.SPhoneContext())
            {
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
