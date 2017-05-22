using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Configuration.Protocols;
using Genesyslab.Platform.Configuration.Protocols.ConfServer.Events;
using Genesyslab.Platform.Configuration.Protocols.ConfServer.Requests.Objects;
using Genesyslab.Platform.Configuration.Protocols.ConfServer.Requests.Security;
using Genesyslab.Platform.Configuration.Protocols.Types;
using log4net;
using SoftPhone.Entity.Model.cfg;
using SoftPhoneService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace SoftPhoneService.SupportClass
{

#pragma warning disable 612, 618

    public class CfgServerHelper
    {
        private readonly static ILog log = LogManager.GetLogger("CfgServer");

        #region 私有变量
        private static string CONFIGSERVER_IDENTIFIER = "localhost_2020_confserv";
        private static ProtocolManagementService protocolManagementService;
        private static ConfServerConfiguration confServerConfiguration;
        private static EventBrokerService eventBrokerService;
        private static IConfService confService;

        private static bool IsStart = false;
        private static bool IsEnd = false;
        #endregion

        #region Genesys Event Handlers
        private static void eventConfServerEventsHandler(IMessage response)
        {
            try
            {
                switch (response.Id)
                {
                    case EventNotificationRegistered.MessageId:
                        {
                            var r = (EventNotificationRegistered)response;
                        }
                        break;
                    //更新
                    case EventObjectUpdated.MessageId:
                        {
                            var r = (EventObjectUpdated)response;
                            if (r.ObjectType == (int)CfgObjectType.CFGPerson)//坐席队列发生变化
                            {
                                var xml = r.ConfObject;
                                var ns = xml.Root.Name.Namespace;
                                var deltaPerson = xml.Root.Element(ns + "CfgDeltaPerson");
                                if (deltaPerson != null)
                                {
                                    var person = deltaPerson.Element(ns + "CfgPerson");
                                    if (person != null)
                                    {
                                        var personDBID = int.Parse(person.Element(ns + "DBID").Attribute("value").Value);//Person.DBID

                                        var adds = new List<PersonSkillNotification>();//增加的
                                        var deleted = new List<PersonSkillNotification>();//删除的
                                        var changed = new List<PersonSkillNotification>();//更改的

                                        var agent = person.Element(ns + "CfgAgentInfo");
                                        if (agent != null)
                                        {
                                            //新增的队列级别
                                            var ele = agent.Element(ns + "skillLevels");
                                            if (ele != null)
                                            {
                                                adds = (from t in ele.Elements(ns + "CfgSkillLevel")
                                                        select new PersonSkillNotification
                                                        {
                                                            skillDBID = int.Parse(t.Element(ns + "skillDBID").Attribute("value").Value),
                                                            level = int.Parse(t.Element(ns + "level").Attribute("value").Value)
                                                        }).ToList();
                                            }
                                        }

                                        var deltaAgentInfo = deltaPerson.Element(ns + "CfgDeltaAgentInfo");
                                        if (deltaAgentInfo != null)
                                        {
                                            var ele = deltaAgentInfo.Element(ns + "deletedSkillDBIDs");
                                            if (ele != null)
                                            {
                                                //删除的队列
                                                deleted = (from t in ele.Elements(ns + "DBID")
                                                           select new PersonSkillNotification
                                                           {
                                                               skillDBID = int.Parse(t.Attribute("value").Value),
                                                               level = 0
                                                           }).ToList();
                                            }

                                            //改变的队列
                                            ele = deltaAgentInfo.Element(ns + "changedSkillLevels");
                                            if (ele != null)
                                            {
                                                changed = (from t in ele.Elements(ns + "CfgSkillLevel")
                                                           select new PersonSkillNotification
                                                           {
                                                               skillDBID = int.Parse(t.Element(ns + "skillDBID").Attribute("value").Value),
                                                               level = int.Parse(t.Element(ns + "level").Attribute("value").Value)
                                                           }).ToList();
                                            }
                                        }

                                        StatServerHelper.UpdateAgentSkills(personDBID, adds, UpdateAgentSkillType.Add);
                                        StatServerHelper.UpdateAgentSkills(personDBID, deleted, UpdateAgentSkillType.Delete);
                                        StatServerHelper.UpdateAgentSkills(personDBID, changed, UpdateAgentSkillType.Update);
                                    }
                                }
                            }

                        }
                        break;

                    //删除
                    case EventObjectDeleted.MessageId:
                        {
                            var r = (EventObjectDeleted)response;
                            if (r.ObjectType == (int)CfgObjectType.CFGSkill)
                            {
                                var dbid = r.Dbid;
                                StatServerHelper.SkillDeleted(dbid);
                            }
                        }
                        break;

                    //新增
                    case EventObjectCreated.MessageId:
                        {
                            var r = (EventObjectCreated)response;
                            if (r.ObjectType == (int)CfgObjectType.CFGSkill)
                            {
                                var xml = r.ConfObject;
                                var ns = xml.Root.Name.Namespace;
                                var cfgSkill = xml.Root.Element(ns + "CfgSkill");
                                if (cfgSkill != null)
                                {
                                    var dbid = int.Parse(cfgSkill.Element(ns + "DBID").Attribute("value").Value);
                                    var name = cfgSkill.Element(ns + "name").Attribute("value").Value;
                                    //create
                                    StatServerHelper.SkillCreated(dbid, name);
                                }
                            }
                        }
                        break;
                }
                log.Debug(response.ToString());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

        }
        private static void OnProtocolOpened(object sender, ProtocolEventArgs e)
        {
            log.Warn("ProtocolOpened");
        }
        private static void OnProtocolClosed(object sender, ProtocolEventArgs e)
        {
            log.Warn("ProtocolClosed");
        }
        #endregion

        #region Private Methods

        private static void InitializePSDKApplicationBlocks()
        {
            // Setup Application Blocks:

            // Create Protocol Manager Service object
            protocolManagementService = new ProtocolManagementService();

            // Create and initialize connection configuration object for TServer.
            confServerConfiguration = new ConfServerConfiguration(CONFIGSERVER_IDENTIFIER);

            var ConfServerURI = WebConfigurationManager.AppSettings["CfgServer.URI"];
            var ClientName = WebConfigurationManager.AppSettings["CfgServer.ClientName"];
            var UserName = WebConfigurationManager.AppSettings["CfgServer.UserName"];
            var UserPassword = WebConfigurationManager.AppSettings["CfgServer.UserPassword"];

            // Set required values
            try
            {
                confServerConfiguration.Uri = new Uri(ConfServerURI);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            confServerConfiguration.ClientName = ClientName;
            confServerConfiguration.UserName = UserName;
            confServerConfiguration.UserPassword = UserPassword;


            if (System.Web.Configuration.WebConfigurationManager.AppSettings["CfgServer.HASupport"] == "1")
            {
                var uriBakup = System.Web.Configuration.WebConfigurationManager.AppSettings["CfgServer.URI_BAKUP"];
                confServerConfiguration.WarmStandbyAttempts = 3;
                confServerConfiguration.WarmStandbyTimeout = 3000;
                confServerConfiguration.WarmStandbyUri = new Uri(uriBakup);
                confServerConfiguration.FaultTolerance = FaultToleranceMode.WarmStandby;
            }

            // Register this connection configuration with Protocol Manager
            protocolManagementService.Register(confServerConfiguration);

            protocolManagementService.ProtocolOpened += new EventHandler<ProtocolEventArgs>(OnProtocolOpened);
            protocolManagementService.ProtocolClosed += new EventHandler<ProtocolEventArgs>(OnProtocolClosed);

            // Create and Initialize Message Broker Application Block
            eventBrokerService = new EventBrokerService(protocolManagementService.Receiver);

            // Activate event broker service 
            eventBrokerService.Activate();

            confService = ConfServiceFactory.CreateConfService(
                protocolManagementService[CONFIGSERVER_IDENTIFIER],
                eventBrokerService);

            eventBrokerService.Register(
              eventConfServerEventsHandler,
              new MessageFilter(protocolManagementService[CONFIGSERVER_IDENTIFIER].ProtocolId));
        }

        private static void FinalizePSDKApplicationBlocks()
        {
            ConfServiceFactory.ReleaseConfService(confService);

            // Cleanup code
            eventBrokerService.Deactivate();

            eventBrokerService.Dispose();

            // Close Connection if opened (check status of protocol object)
            IProtocol protocol = protocolManagementService[CONFIGSERVER_IDENTIFIER];

            if (protocol.State == ChannelState.Opened) // Close only if the protocol state is opened
            {
                try
                {
                    protocol.Close();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
            }
            protocolManagementService.Unregister(CONFIGSERVER_IDENTIFIER);
        }

        private static IConfService OpenCfg()
        {
            var ConfServerURI = WebConfigurationManager.AppSettings["CfgServer.URI"];
            var ClientName = WebConfigurationManager.AppSettings["CfgServer.ClientName"];
            var UserName = WebConfigurationManager.AppSettings["CfgServer.UserName"];
            var UserPassword = WebConfigurationManager.AppSettings["CfgServer.UserPassword"];

            Endpoint confServerUri = new Endpoint(new Uri(ConfServerURI));
            ConfServerProtocol cfgServiceProtocol;
            EventBrokerService _eventBrokerService;

            cfgServiceProtocol = new ConfServerProtocol(confServerUri);
            cfgServiceProtocol.ClientName = ClientName;
            cfgServiceProtocol.UserName = UserName;
            cfgServiceProtocol.UserPassword = UserPassword;
            cfgServiceProtocol.ClientApplicationType = (int)CfgAppType.CFGSCE;

            try
            {
                cfgServiceProtocol.Open();
            }
            catch (ProtocolException e)
            {
                log.Error(e.Message);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }

            _eventBrokerService = BrokerServiceFactory.CreateEventBroker(cfgServiceProtocol);

            IConfService cfgService = ConfServiceFactory.CreateConfService(cfgServiceProtocol, _eventBrokerService);

            return cfgService;
        }

        private static void CloseCfg(IConfService _confService)
        {
            ConfServiceFactory.ReleaseConfService(_confService);
        }
        #endregion

        #region GetProtocol
        private static IProtocol GetProtocol()
        {
            if (!IsStart)
            {
                throw new Exception("服务未启动");
            }
            if (IsEnd)
            {
                throw new Exception("服务已结束");
            }
            IProtocol protocol = protocolManagementService[CONFIGSERVER_IDENTIFIER];
            if (!IsEnd)
            {
                if (protocol.State == ChannelState.Closed)
                {
                    protocol.Open(new TimeSpan(0, 0, 10));
                }
            }
            return protocol;
        }
        #endregion

        #region ==============启动、结束==============
        internal static void Start()
        {
            try
            {
                log.Info("CfgServer.Start 开始");
                InitializePSDKApplicationBlocks();
                IsStart = true;
                try
                {
                    var cfg = GetProtocol();
                    if (cfg.State == ChannelState.Closed)
                    {
                        cfg.BeginOpen();
                    }
                    Connect();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
                RegisterNotification(3);
                RegisterNotification(13);
                log.Info("CfgServer.Start 成功");
            }
            catch (Exception ex)
            {
                log.Info("CfgServer.Start 失败");
                log.Error(ex.Message, ex);
            }
        }

        internal static void End()
        {
            try
            {
                log.Info("CfgServer.End 开始");
                UnregisterNotification(3);
                UnregisterNotification(13);
                System.Threading.Thread.Sleep(500);
                GetProtocol().BeginClose();
                IsEnd = true;
                FinalizePSDKApplicationBlocks();
                log.Info("CfgServer.End 成功");
            }
            catch (Exception ex)
            {
                log.Info("CfgServer.End 失败");
                log.Error(ex.Message, ex);
            }
        }
        #endregion

        #region 【sdk】连接、注册通知、注销通知
        private static void Connect()
        {
            // Open the connection - only when the connection is not already opened
            // Opening the connection can fail and raises an exception
            try
            {
                IProtocol protocol = protocolManagementService[CONFIGSERVER_IDENTIFIER];

                if (protocol.State == ChannelState.Closed)
                    protocol.Open(); // attempt to open the channel asynchronously

            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        private static void RegisterNotification(int object_type)
        {

            try
            {
                // Create the subscribe request for MessageServer (Management Platform SDK). 
                RequestRegisterNotification request = RequestRegisterNotification.Create();

                // Note: Subscriptions KeyValue collection contains all subscriptions we want to define for this request.
                // The KeyValue collections that actually defines the subscription are at second level.
                // Two-dimensional TKVList containing lists with the following properties (the names of the keys on the first level 
                // are disregarded let them just be unique): 
                // object_type - type of the objects to receive notifications on (if equals to 0 means all the object types), 
                // tenant_id - Tenant affiliation of the objects to receive notifications on (if equals to 0 means all the Tenants), 
                // object_dbid - DBID of the specific object to receive notifications on (used in conjunction with object_type), 
                // exclude_bytecode - if set, Configuration Server will filter out binary userProperties field 'bytecode' from notifications.
                KeyValueCollection subscriptionForAllObjects = new KeyValueCollection();
                subscriptionForAllObjects.Add("object_type", object_type);
                subscriptionForAllObjects.Add("tenant_id", 0);
                subscriptionForAllObjects.Add("object_dbid", 0);


                KeyValueCollection topLevelCollection = new KeyValueCollection();
                topLevelCollection.Add("subscriptionForAllObjects", subscriptionForAllObjects);

                request.Subscription = topLevelCollection;

                log.Debug(request.ToString());

                // Request asynchronously
                GetProtocol().Send(request);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        private static void UnregisterNotification(int object_type)
        {
            try
            {
                // Create the subscribe request for MessageServer (Management Platform SDK). 
                RequestUnregisterNotification request = RequestUnregisterNotification.Create();

                // Note: Subscriptions KeyValue collection contains all subscriptions we want to define for this request.
                // The KeyValue collections that actually defines the subscription are at second level.
                // Two-dimensional TKVList containing lists with the following properties (the names of the keys on the first level 
                // are disregarded let them just be unique): 
                // object_type - type of the objects to receive notifications on (if equals to 0 means all the object types), 
                // tenant_id - Tenant affiliation of the objects to receive notifications on (if equals to 0 means all the Tenants), 
                // object_dbid - DBID of the specific object to receive notifications on (used in conjunction with object_type), 
                // exclude_bytecode - if set, Configuration Server will filter out binary userProperties field 'bytecode' from notifications.
                KeyValueCollection subscriptionForAllObjects = new KeyValueCollection();
                subscriptionForAllObjects.Add("object_type", object_type);
                subscriptionForAllObjects.Add("tenant_id", 0);
                subscriptionForAllObjects.Add("object_dbid", 0);


                KeyValueCollection topLevelCollection = new KeyValueCollection();
                topLevelCollection.Add("subscriptionForAllObjects", subscriptionForAllObjects);

                request.Subscription = topLevelCollection;

                log.Debug(request.ToString());

                // Request asynchronously
                GetProtocol().Send(request);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }
        #endregion

        #region 验证用户名密码
        /// <summary>
        /// 验证用户名密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userPassword">密码</param>
        /// <returns>Models.LoginResult</returns>
        public static LoginResult Authenticate(string userName, string userPassword)
        {
            var result = new LoginResult();
            var icfg = OpenCfg();
            try
            {
                RequestAuthenticate request = RequestAuthenticate.Create(userName, userPassword);
                IMessage msg = icfg.Protocol.Request(request, new TimeSpan(0, 0, 5));
                if (msg.Name == "EventAuthenticated")
                {
                    result.EventAuthenticated = true;
                }
                else if (msg.Name == "EventError")
                {
                    var ev = (EventError)msg;
                    result.ErrorMessage = ev.Description;
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                CloseCfg(icfg);
            }
            return result;
        }
        #endregion

        #region 获取员工（坐席、管理员）、坐席信息、坐席能力 GetPerson
        /// <summary>
        /// 获取员工（坐席、管理员）、坐席信息、坐席能力
        /// </summary>
        /// <param name="employeeId">员工编号</param>
        public static Person GetPerson(string employeeId)
        {
            var result = new List<Person>();
            var icfg = OpenCfg();
            try
            {
                CfgPersonQuery qPerson1 = new CfgPersonQuery(icfg);
                qPerson1.TenantDbid = 101;
                qPerson1.State = CfgObjectState.CFGEnabled;
                qPerson1.IsAgent = 2;//只有坐席能登录
                qPerson1.EmployeeId = employeeId;
                var persons = qPerson1.Execute();
                foreach (var person1 in persons)
                {
                    var info = new Person();
                    info.DBID = person1.DBID;
                    info.EmployeeID = person1.EmployeeID;
                    info.FirstName = person1.FirstName;
                    info.UserName = person1.UserName;
                    if (person1.AgentInfo != null)
                    {
                        info.AgentInfo.IsInitAgentInfo = true;
                        //坐席信息
                        foreach (var item in person1.AgentInfo.AgentLogins)
                        {
                            if (item.AgentLogin != null && item.AgentLogin.State == CfgObjectState.CFGEnabled)
                            {
                                info.LoginCode = item.AgentLogin.LoginCode;
                                info.AgentInfo.AgentLogins.Add(new AgentLogin()
                                {
                                    DBID = item.AgentLogin.DBID,
                                    LoginCode = item.AgentLogin.LoginCode,
                                });
                            }
                        }

                        //队列
                        foreach (var item in person1.AgentInfo.SkillLevels)
                        {
                            if (item.Skill != null && item.Skill.State == CfgObjectState.CFGEnabled)
                            {
                                info.AgentInfo.SkillLevels.Add(new SkillLevel() { DBID = item.Skill.DBID, Level = item.Level });
                            }
                        }
                    }

                    //服务能力
                    if (person1.AgentInfo != null)
                    {
                        info.VOICE = 1;
                        if (person1.AgentInfo.CapacityRule != null)
                        {
                            CfgScript script = person1.AgentInfo.CapacityRule;
                            if (script != null)
                            {
                                KeyValueCollection kv = script.UserProperties;
                                if (kv != null && kv.Get("_CRWizardMediaCapacityList_") != null)
                                {
                                    KeyValueCollection kv1 = (KeyValueCollection)kv.Get("_CRWizardMediaCapacityList_");
                                    foreach (var key in kv1.AllKeys)
                                    {
                                        if (key == "chat")
                                        {
                                            info.CHAT = kv1.GetAsInt(key);
                                        }
                                        //else if (key == "voice")
                                        //{
                                        //    info.VOICE = kv1.GetAsInt(key);
                                        //}
                                    }
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(info.LoginCode))
                    {
                        info.LoginCode = "";
                    }
                    result.Add(info);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseCfg(icfg);
            }
            if (result.Count > 0)
            {
                var person = result.First();
                var ip = HttpContext.Current.Request.UserHostAddress;
                using (var db = new Tele.DataLibrary.SPhoneEntities())
                {
                    var ipdn = db.SPhone_IPDN.Where(x => x.PlaceIP == ip).FirstOrDefault();
                    if (ipdn != null)
                    {
                        person.DN = ipdn.DNNo;
                        person.Place = ipdn.Place;
                    }
                }
                return person;
            }
            return null;
        }
        #endregion

        #region 获取所有员工。不包含坐席信息，坐席能力 GetPersonsShort
        /// <summary>
        /// 获取所有员工。不包含坐席信息，坐席能力
        /// </summary>
        /// <returns></returns>
        public static List<Person> GetPersonsShort()
        {
            var result = new List<Person>();
            var icfg = OpenCfg();
            try
            {
                CfgPersonQuery qPerson1 = new CfgPersonQuery(icfg);
                qPerson1.TenantDbid = 101;
                qPerson1.State = CfgObjectState.CFGEnabled;
                qPerson1.IsAgent = 2;
                var persons = qPerson1.Execute();
                foreach (var person1 in persons)
                {
                    var info = new Person();
                    info.DBID = person1.DBID;
                    info.EmployeeID = person1.EmployeeID;
                    info.FirstName = person1.FirstName;
                    info.UserName = person1.UserName;
                    result.Add(info);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseCfg(icfg);
            }
            return result;
        }
        #endregion

        #region 获取员工编号对应的UserName
        /// <summary>
        /// 获取员工编号对应的UserName
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public static string GetPersonUserName(string employeeId)
        {
            var icfg = OpenCfg();
            try
            {
                CfgPersonQuery qPerson1 = new CfgPersonQuery(icfg);
                qPerson1.TenantDbid = 101;
                qPerson1.EmployeeId = employeeId;
                qPerson1.State = CfgObjectState.CFGEnabled;
                //qPerson1.IsAgent = 2;
                var person = qPerson1.ExecuteSingleResult();

                if (person != null)
                {
                    return person.UserName;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseCfg(icfg);
            }
            return string.Empty;
        }
        #endregion

        #region 获取所有员工（坐席）、坐席信息、坐席能力 此方法效率太慢,GetPersons
        /// <summary>
        /// 获取所有员工（坐席）、坐席信息、坐席能力
        /// 此方法效率太慢
        /// </summary>
        /// <returns></returns>
        public static List<Person> GetPersons()
        {
            var result = new List<Person>();
            var icfg = OpenCfg();
            try
            {
                CfgPersonQuery qPerson1 = new CfgPersonQuery(icfg);
                qPerson1.TenantDbid = 101;
                qPerson1.State = CfgObjectState.CFGEnabled;
                qPerson1.IsAgent = 2;
                var persons = qPerson1.Execute();
                foreach (var person1 in persons)
                {
                    var info = new Person();
                    info.DBID = person1.DBID;
                    info.EmployeeID = person1.EmployeeID;
                    info.FirstName = person1.FirstName;
                    info.UserName = person1.UserName;
                    if (person1.AgentInfo != null)
                    {
                        info.AgentInfo.IsInitAgentInfo = true;
                        //坐席信息
                        foreach (var item in person1.AgentInfo.AgentLogins)
                        {
                            if (item.AgentLogin != null && item.AgentLogin.State == CfgObjectState.CFGEnabled)
                            {
                                info.AgentInfo.AgentLogins.Add(new AgentLogin()
                                {
                                    DBID = item.AgentLogin.DBID,
                                    LoginCode = item.AgentLogin.LoginCode,
                                });
                            }
                        }

                        //队列
                        foreach (var item in person1.AgentInfo.SkillLevels)
                        {
                            if (item.Skill != null && item.Skill.State == CfgObjectState.CFGEnabled)
                            {
                                info.AgentInfo.SkillLevels.Add(new SkillLevel() { DBID = item.Skill.DBID, Level = item.Level });
                            }
                        }
                    }

                    //服务能力
                    if (person1.AgentInfo != null)
                    {
                        info.VOICE = 1;
                        if (person1.AgentInfo.CapacityRule != null)
                        {
                            CfgScript script = person1.AgentInfo.CapacityRule;
                            if (script != null)
                            {
                                KeyValueCollection kv = script.UserProperties;
                                if (kv != null && kv.Get("_CRWizardMediaCapacityList_") != null)
                                {
                                    KeyValueCollection kv1 = (KeyValueCollection)kv.Get("_CRWizardMediaCapacityList_");
                                    foreach (var key in kv1.AllKeys)
                                    {
                                        if (key == "chat")
                                        {
                                            info.CHAT = kv1.GetAsInt(key);
                                        }
                                        else if (key == "voice")
                                        {
                                            info.VOICE = kv1.GetAsInt(key);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    result.Add(info);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseCfg(icfg);
            }
            return result;
        }
        #endregion

        #region 获取队列Number号 GetSwitcheDNs
        /// <summary>
        /// 获取队列Number号
        /// </summary>
        /// <returns></returns>
        public static List<SwitcheVQ> GetSwitcheDNs(string name = null)
        {
            var result = new List<SwitcheVQ>();

            var key = @"\Configuration\Resources\Switches\";
            var icfg = OpenCfg();
            try
            {
                CfgDNQuery query = new CfgDNQuery(icfg);
                query.State = CfgObjectState.CFGEnabled;
                if (name != null)
                {
                    query.Name = name;
                }
                var dns = query.Execute();

                foreach (var dn in dns.ToList().Where(x => !string.IsNullOrEmpty(x.Name)))
                {
                    var objpath = dn.ObjectPath;
                    if (objpath.StartsWith(key))
                    {
                        var arr = objpath.Substring(key.Length).Split('\\');
                        if (arr.Length > 0)
                        {
                            var info = new SwitcheVQ();
                            info.SwitchName = arr[0];
                            info.Number = dn.Number;
                            info.DnName = dn.Name;
                            result.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseCfg(icfg);
            }
            return result;
        }
        #endregion

        #region 获取所有队列 GetSkills
        /// <summary>
        /// 获取所有队列,dbid>0时会带入查询条件
        /// </summary>
        public static List<Skill> GetSkills(int? dbid = null, string name = null)
        {
            var result = new List<Skill>();
            var dns = new List<SwitcheVQ>();
            if (name == null)
            {
                dns = GetSwitcheDNs(name);
            }
            else
            {
                dns = GetSwitcheDNs("V" + name);
            }
            var icfg = OpenCfg();
            try
            {
                CfgSkillQuery query = new CfgSkillQuery(icfg);
                query.State = CfgObjectState.CFGEnabled;
                if (dbid.HasValue)
                {
                    query.Dbid = dbid.Value;
                }
                var skills = query.Execute();
                foreach (var item in skills.ToList())
                {
                    var info = new Skill()
                    {
                        DBID = item.DBID,
                        Name = item.Name,
                        ObjectPath = item.ObjectPath,
                        LastDate = DateTime.Now,
                        LastValue = 0,
                    };

                    if (info.Name.StartsWith("Q"))
                    {
                        var dn = dns.Where(x => x.DnName.ToLower() == ("V" + info.Name).ToLower()).FirstOrDefault();
                        if (dn != null)
                        {
                            info.Number = dn.Number;
                            info.SwitchName = dn.SwitchName;
                            result.Add(info);
                        }
                    }
                    else if (info.Name.StartsWith("VQ"))
                    {
                        var dn = dns.Where(x => x.DnName.ToLower() == (info.Name).ToLower()).FirstOrDefault();
                        if (dn != null)
                        {
                            info.Number = dn.Number;
                            info.SwitchName = dn.SwitchName;
                            result.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseCfg(icfg);
            }
            return result;
        }
        #endregion
    }

#pragma warning restore 612, 618
}