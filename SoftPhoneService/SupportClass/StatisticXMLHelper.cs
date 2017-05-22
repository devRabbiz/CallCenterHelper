using SoftPhone.Entity.Model.cfg;
using SoftPhone.Entity.Model.stat;
using SoftPhoneService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SoftPhoneService.SupportClass
{
    public class StatisticXMLHelper
    {
        public static XElement _doc;

        public static XElement doc
        {
            get
            {
                if (_doc == null)
                {
                    _doc = XElement.Load(HttpContext.Current.Server.MapPath("~/Statistic.xml"));
                }
                return _doc;
            }
        }

        #region GetHASupports
        public static List<HASupport> GetHASupports()
        {
            List<HASupport> list = new List<HASupport>();
            IEnumerable<XElement> childList = from el in doc.Elements("HASupport").Elements("item")
                                              select el;
            foreach (var cl in childList)
            {
                var info = new HASupport();
                if (cl.Attribute("Key") != null)
                {
                    info.Key = cl.Attribute("Key").Value;
                }
                if (cl.Attribute("HA") != null)
                {
                    info.HA = cl.Attribute("HA").Value == "1";
                }
                if (cl.Attribute("BacupURI") != null)
                {
                    info.BacupURI = cl.Attribute("BacupURI").Value;
                }
            }
            return list;
        }
        #endregion

        #region 获取xml配置
        /// <summary>
        /// 格式化xml配置
        /// </summary>
        /// <param name="persons"></param>
        /// <returns></returns>
        public static List<StatisticItem> GetStatisticItems(List<Person> persons, List<Skill> skills)
        {
            List<StatisticItem> list = new List<StatisticItem>();
            IEnumerable<XElement> childList = from el in doc.Elements("Statistic")
                                              select el;
            int i = 0;
            foreach (XElement cl in childList)
            {
                i++;
                var info = new Statistic();
                if (cl.Attribute("CacheKey") != null)
                    info.CacheKey = cl.Attribute("CacheKey").Value;
                if (cl.Attribute("StatisticType") != null)
                    info.StatisticType = cl.Attribute("StatisticType").Value;
                if (cl.Attribute("ObjectType") != null)
                    info.ObjectType = int.Parse(cl.Attribute("ObjectType").Value);
                if (cl.Attribute("Uri") != null)
                    info.Uri = cl.Attribute("Uri").Value;
                if (cl.Attribute("BaseReferenceId") != null)
                    info.BaseReferenceId = int.Parse(cl.Attribute("BaseReferenceId").Value);
                info.Index = i;
                info.ClientName = "SDI2-" + info.Uri;
                if (info.CacheKey.StartsWith("skill/"))
                {
                    var switchname = info.CacheKey.Substring("skill/".Length);
                    var skillquery = skills.Where(x => x.SwitchName == switchname).ToList();
                    foreach (var skill in skillquery)
                    {
                        var item = new StatisticItem()
                        {
                            ObjectId = skill.Number + "@" + skill.SwitchName,
                            DBID = skill.DBID,
                            TypeID = 1
                        };
                        item.ReferenceId = item.DBID + info.BaseReferenceId;
                        item.Statistic = info;
                        list.Add(item);
                    }
                }
                else if (info.CacheKey.StartsWith("agent/"))
                {
                    if (persons != null)
                    {
                        foreach (var person in persons)
                        {
                            var item = new StatisticItem()
                            {
                                ObjectId = person.EmployeeID,
                                DBID = person.DBID,
                                TypeID = 2
                            };
                            item.ReferenceId = item.DBID + info.BaseReferenceId;
                            item.Statistic = info;
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }
        #endregion
    }
}