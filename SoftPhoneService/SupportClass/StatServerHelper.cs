using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Configuration.Protocols;
using Genesyslab.Platform.Reporting.Protocols.StatServer;
using Genesyslab.Platform.Reporting.Protocols.StatServer.Events;
using Genesyslab.Platform.Reporting.Protocols.StatServer.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftPhone.Entity.Model.cfg;
using System.Threading;
using SoftPhone.Entity.Model.stat;
using System.Collections.Concurrent;
using System.IO;

namespace SoftPhoneService.SupportClass
{

#pragma warning disable 612, 618
    /******************************************
     * 数据范围规定 总9位 前2位代表业务
     ******************************************
         
        队列排队量：    [11]Current_In_Queue
        呼入量：        [12]TotalNumberInboundCalls
        呼出量：        [13]TotalNumberOutboundCalls
        Chat处理总量：  [14]Total_Inbound_Handled
        (呼出AHT：       [15]AverOutboundStatusTime [去掉了，暂时不要])
        呼入AHT：（案面时长+通话时长）/呼入数量
                        ([16]Total_Work_Time + [17]Total_Talk_Time_CC)/ [12]TotalNumberInboundCalls+[13]TotalNumberOutboundCalls
        坐席状态：      [18]CurrentAgentState
     * 
     * 凌晨24点清理 - > CloseStatistic(11,12,13,14,15,16,17,18) -> Open(11)
     */
    public class StatServerHelper
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("StatServer");

        #region 私有变量
        private static bool IsReInit = false;//01：00时，是否正在重新初始化

        private static ProtocolManagementService protocolManagementService;
        private static EventBrokerService eventBrokerService;
        private static bool IsStart;

        // How frequently we want to receive statistics
        private static int NOTIFICATION_FREQUENCY = 3;
        private static String TENANT_NAME = "Resources";
        private static bool IsEnd;
        //private static Thread t1, t2, t3, t4;
        private static bool IsAutoReInit;
        #endregion

        #region 公共变量 List类型
        /// <summary>
        /// 所有——订阅Item
        /// </summary>
        internal static ConcurrentDictionary<long, StatisticItem> ALLStatisticItems = new ConcurrentDictionary<long, StatisticItem>();//所有——订阅Item

        /// <summary>
        /// 配置所需的——状态服务器
        /// </summary>
        internal static List<Statistic> ALLStatServer = new List<Statistic>();//配置所需的——状态服务器

        /// <summary>
        /// 订阅Item的父 集合
        /// </summary>
        internal static List<Statistic> ALLStatistics = new List<Statistic>();//订阅Item的父 集合

        internal static List<Skill> ALLSkills = new List<Skill>();//所有——队列
        internal static List<Person> ALLPersons = new List<Person>();//所有——人员

        /// <summary>
        /// 所有热备配置
        /// </summary>
        internal static List<Models.HASupport> AllHASupports = new List<Models.HASupport>();
        #endregion

        #region 初始化、卸载、启动、关闭、启动队列、关闭队列、每天初始化
        private static void InitializePSDKApplicationBlocks()
        {
            try
            {
                protocolManagementService = new ProtocolManagementService();
                eventBrokerService = new EventBrokerService(protocolManagementService.Receiver);

                int i = 0;
                foreach (var s in ALLStatServer)
                {
                    i++;
                    var statServerConfiguration = new StatServerConfiguration(s.ClientName);
                    statServerConfiguration.Uri = new Uri(s.Uri);
                    statServerConfiguration.ClientName = s.ClientName;

                    var ha = AllHASupports.Where(x => x.Key == s.Uri).FirstOrDefault();
                    if (ha != null && ha.HA)
                    {
                        statServerConfiguration.WarmStandbyAttempts = 3;
                        statServerConfiguration.WarmStandbyTimeout = 3000;
                        statServerConfiguration.WarmStandbyUri = new Uri(ha.BacupURI);
                        statServerConfiguration.FaultTolerance = FaultToleranceMode.WarmStandby;
                    }

                    protocolManagementService.Register(statServerConfiguration);

                    #region 最多注册10个，以后可以增加
                    if (i == 1)
                    {
                        eventBrokerService.Register(
                            StatServerEventsHandler1,
                            new MessageFilter(protocolManagementService[s.ClientName].ProtocolId)
                            );
                    }
                    else if (i == 2)
                    {
                        eventBrokerService.Register(
                                StatServerEventsHandler2,
                                new MessageFilter(protocolManagementService[s.ClientName].ProtocolId)
                                );
                    }
                    else if (i == 3)
                    {
                        eventBrokerService.Register(
                                StatServerEventsHandler3,
                                new MessageFilter(protocolManagementService[s.ClientName].ProtocolId)
                                );
                    }
                    else if (i == 4)
                    {
                        eventBrokerService.Register(
                                StatServerEventsHandler4,
                                new MessageFilter(protocolManagementService[s.ClientName].ProtocolId)
                                );
                    }
                    else if (i == 5)
                    {
                        eventBrokerService.Register(
                                StatServerEventsHandler5,
                                new MessageFilter(protocolManagementService[s.ClientName].ProtocolId)
                                );
                    }
                    else if (i == 6)
                    {
                        eventBrokerService.Register(
                                StatServerEventsHandler6,
                                new MessageFilter(protocolManagementService[s.ClientName].ProtocolId)
                                );
                    }
                    else if (i == 7)
                    {
                        eventBrokerService.Register(
                                StatServerEventsHandler7,
                                new MessageFilter(protocolManagementService[s.ClientName].ProtocolId)
                                );
                    }
                    else if (i == 8)
                    {
                        eventBrokerService.Register(
                                StatServerEventsHandler8,
                                new MessageFilter(protocolManagementService[s.ClientName].ProtocolId)
                                );
                    }
                    else if (i == 9)
                    {
                        eventBrokerService.Register(
                                StatServerEventsHandler9,
                                new MessageFilter(protocolManagementService[s.ClientName].ProtocolId)
                                );
                    }
                    else if (i == 10)
                    {
                        eventBrokerService.Register(
                                StatServerEventsHandler10,
                                new MessageFilter(protocolManagementService[s.ClientName].ProtocolId)
                                );
                    }
                    #endregion
                }
                protocolManagementService.ProtocolOpened += new EventHandler<ProtocolEventArgs>(OnProtocolOpened);
                protocolManagementService.ProtocolClosed += new EventHandler<ProtocolEventArgs>(OnProtocolClosed);
                eventBrokerService.Activate();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        private static void FinalizePSDKApplicationBlocks()
        {
            eventBrokerService.Deactivate();
            eventBrokerService.Dispose();
            foreach (var s in ALLStatServer)
            {
                IProtocol protocol = protocolManagementService[s.ClientName];
                if (protocol.State == ChannelState.Opened)
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
                protocolManagementService.Unregister(s.ClientName);
            }
        }

        private static void OnProtocolOpened(object sender, ProtocolEventArgs e)
        {
            log.Info("ProtocolOpened");
        }

        private static void OnProtocolClosed(object sender, ProtocolEventArgs e)
        {
            log.Info("ProtocolClosed");
        }

        private static void Send(StatisticItem item, IMessage message)
        {
            if (!IsStart)
            {
                throw new Exception("服务未启动");
            }
            if (IsEnd)
            {
                throw new Exception("服务已结束，无法查询");
            }
            try
            {
                var p = protocolManagementService[item.Statistic.ClientName];
                if (p.State == ChannelState.Closed)
                {
                    p.Open(new TimeSpan(0, 0, 5));
                }
                p.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private static void PreInit()
        {
            #region 重置变量
            ALLPersons = new List<Person>();
            AllHASupports = new List<Models.HASupport>();
            ALLSkills = new List<Skill>();
            ALLStatisticItems = new ConcurrentDictionary<long, StatisticItem>();
            ALLStatistics = new List<Statistic>();
            ALLStatServer = new List<Statistic>();
            #endregion

            ALLPersons = CfgServerHelper.GetPersonsShort(); //1.先初始化人员（短）
            AllHASupports = SupportClass.StatisticXMLHelper.GetHASupports();//2.获取所有热备 
            ALLSkills = SupportClass.CfgServerHelper.GetSkills();//3.获取所有队列
            var statisticItems = SupportClass.StatisticXMLHelper.GetStatisticItems(ALLPersons, ALLSkills);//4.根据person和队列格式化订阅item
            statisticItems.ForEach(x => ALLStatisticItems.TryAdd(x.ReferenceId, x));

            //所有配置
            var query1 = ALLStatisticItems.Values.GroupBy(x => x.Statistic.Index).ToList();
            foreach (var item in query1)
            {
                ALLStatistics.Add((Statistic)item.First().Statistic.Clone());
            }

            //配置服务器
            var query2 = ALLStatistics.GroupBy(x => x.Uri).ToList();
            foreach (var item in query2)
            {
                ALLStatServer.Add((Statistic)item.First().Clone());
            }
        }

        internal static void Start()
        {
            try
            {
                if (!IsStart)
                {
                    log.Info("Start.Begin");

                    IsAutoReInit = System.Web.Configuration.WebConfigurationManager.AppSettings["AutoReInit.stat"] == "1";

                    PreInit();//初始化数据

                    InitializePSDKApplicationBlocks();//初始化

                    IsStart = true;
                    IsEnd = false;

                    foreach (var s in ALLStatServer)
                    {
                        var p = protocolManagementService[s.ClientName];
                        p.BeginOpen();
                    }

                    ThreadPool.QueueUserWorkItem(new WaitCallback(OpenQueneStatistic), null);

                    //01:00重新Close-Open

                    if (IsAutoReInit)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(ReInit), null);
                    }
                    //t1 = new Thread(ReInit);
                    //t1.IsBackground = true;
                    //t1.Start();

                    //监控10秒后没有更新
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(CheckLastTime), null);
                    //t2 = new Thread(CheckLastTime);
                    //t2.IsBackground = true;
                    //t2.Start();

                    //监控坐席请求需要打开的订阅
                    ThreadPool.QueueUserWorkItem(new WaitCallback(CheckRequireOpenStatistic), null);
                    //t3 = new Thread(CheckRequireOpenStatistic);
                    //t3.IsBackground = true;
                    //t3.Start();

                    //--监控健康状态
                    //t4 = new Thread(CheckProtocolState);
                    //t4.IsBackground = true;
                    //t4.Start();
                }
                log.Info("Start.Succ");
            }
            catch (Exception ex)
            {
                log.Error("Start.Error", ex);
            }
        }

        internal static void End()
        {
            if (IsStart)
            {
                log.Info("End.Begin");
                try
                {
                    var query = ALLStatisticItems.Values.Where(x => x.Opened).ToList();
                    foreach (var item in query)
                    {
                        CloseStatistic(item);
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
                IsStart = false;
                IsEnd = true;
                try
                {
                    foreach (var s in ALLStatServer)
                    {
                        try
                        {
                            IProtocol protocol = protocolManagementService[s.ClientName];
                            if (protocol.State == ChannelState.Opened)
                            {
                                protocol.BeginClose();
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message, ex);
                        }
                    }

                    FinalizePSDKApplicationBlocks();
                    log.Warn("End.Succ");
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// 打开队列统计
        /// </summary>
        public static void OpenQueneStatistic(Object stateInfo)
        {
            Thread.Sleep(2000);
            var query = ALLStatisticItems.Values.Where(x => x.TypeID == 1).ToList();
            foreach (var item in query)
            {
                item.LastDate = DateTime.Now;
                item.RequireOpen = true;
                OpenStatistic(item);
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// 关闭队列统计
        /// </summary>
        public static void CloseQueneStatistic(Object stateInfo)
        {
            Thread.Sleep(2000);
            var query = ALLStatisticItems.Values.Where(x => x.TypeID == 1 && x.REQ_ID > 0).ToList();
            foreach (var item in query)
            {
                CloseStatistic(item);
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// 初始化01点数据:重新获取初始化数据
        /// 1.skill队列Close，队列Open
        /// 2.person 关闭，清空
        /// </summary>
        internal static void ReInit(Object stateInfo)
        {
            Thread.Sleep(1000);
            while (!IsEnd)
            {
                var current = DateTime.Now.ToString("HH:mm:ss");
                if (current == "01:00:00" || current == "01:00:01" || current == "01:00:02" || current == "01:00:03" || current == "01:00:04" || (stateInfo != null && stateInfo.ToString().ToLower() == "true"))
                {
                    log.Info("[ReInit]");
                    stateInfo = false;
                    IsReInit = true;
                    log.Debug("01点重新初始化:begin");
                    try
                    {
                        log.Debug("01点重新初始化:try");
                        var query = ALLStatisticItems.Values.Where(x => x.REQ_ID > 0).OrderBy(x => x.TypeID).ToList();
                        foreach (var item in query)
                        {
                            item.LastDate = DateTime.Now;
                            CloseStatistic(item);
                            Thread.Sleep(50);
                        }

                        PreInit();//重新初始化数据

                        OpenQueneStatistic(null);
                    }
                    catch (Exception ex)
                    {
                        log.Debug("01点重新初始化:catch");
                        log.Error(ex);
                    }
                    log.Debug("01点重新初始化:end");
                    IsReInit = false;
                    Thread.Sleep(5000);
                }
                Thread.Sleep(500);
            }
        }
        #endregion

        #region ***************************接收事件***************************
        #region 注册10个，以后可以增加
        private static void StatServerEventsHandler1(IMessage response)
        {
            EventsHandler_StatServer(response);
        }
        private static void StatServerEventsHandler2(IMessage response)
        {
            EventsHandler_StatServer(response);
        }
        private static void StatServerEventsHandler3(IMessage response)
        {
            EventsHandler_StatServer(response);
        }
        private static void StatServerEventsHandler4(IMessage response)
        {
            EventsHandler_StatServer(response);
        }
        private static void StatServerEventsHandler5(IMessage response)
        {
            EventsHandler_StatServer(response);
        }
        private static void StatServerEventsHandler6(IMessage response)
        {
            EventsHandler_StatServer(response);
        }
        private static void StatServerEventsHandler7(IMessage response)
        {
            EventsHandler_StatServer(response);
        }
        private static void StatServerEventsHandler8(IMessage response)
        {
            EventsHandler_StatServer(response);
        }
        private static void StatServerEventsHandler9(IMessage response)
        {
            EventsHandler_StatServer(response);
        }
        private static void StatServerEventsHandler10(IMessage response)
        {
            EventsHandler_StatServer(response);
        }
        #endregion
        private static void EventsHandler_StatServer(IMessage response)
        {
            if (IsEnd)
            {
                return;
            }
            switch (response.Id)
            {
                case EventStatisticOpened.MessageId:
                    {
                        try
                        {
                            log.Debug(response.ToString());
                            var r = (EventStatisticOpened)response;
                            var id = r.Tag;
                            if (id > 0 && ALLStatisticItems.ContainsKey(id))
                            {
                                var item = ALLStatisticItems[id];
                                item.REQ_ID = r.ReferenceId;
                                item.Opened = true;
                                item.Closed = false;
                                item.RequireOpen = false;
                                item.LastDate = DateTime.Now;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message, ex);
                        }
                    }
                    break;

                case EventStatisticClosed.MessageId:
                    {
                        try
                        {
                            log.Debug(response.ToString());
                            var r = (EventStatisticClosed)response;
                            var id = r.Tag;
                            if (id != -1 && ALLStatisticItems.ContainsKey(id))
                            {
                                var item = ALLStatisticItems[id];
                                item.REQ_ID = 0;
                                item.Closed = true;
                                item.Opened = false;
                                item.RequireOpen = false;
                                item.LastDate = DateTime.Now;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message, ex);
                        }
                    }
                    break;

                case EventError.MessageId:
                    {
                        var r = (EventInfo)response;
                        //var id = r.Tag;
                        //if (id > 0)
                        //{
                        //    if (ALLStatisticItems.ContainsKey(id))
                        //    {
                        //        var item = ALLStatisticItems[id];
                        //        item.LastDate = DateTime.Now;
                        //        item.REQ_ID = 0;
                        //        item.RequireOpen = true;
                        //        item.Opened = false;
                        //        item.Closed = true;
                        //    }
                        //}
                        log.Error(response.ToString());
                    }
                    break;

                case EventInfo.MessageId:
                    {
                        try
                        {
                            var r = (EventInfo)response;
                            var id = r.Tag;
                            if (id > 0)
                            {
                                var value = 0;
                                if (r.StringValue != null)
                                {
                                    value = int.Parse(r.StringValue);
                                }
                                else if (r.StateValue != null)
                                {
                                    var status = ((Genesyslab.Platform.Reporting.Protocols.StatServer.AgentStatus)r.StateValue).Status;
                                    if (status.HasValue)
                                    {
                                        value = status.Value;
                                    }
                                }
                                if (ALLStatisticItems.ContainsKey(id))
                                {
                                    var item = ALLStatisticItems[id];
                                    if (item.Statistic.CacheKey != "agent/19")
                                    {
                                        item.LastDate = DateTime.Now;
                                        item.LastValue = value;
                                        log.Debug("#EventInfo# TypeID:" + item.TypeID + " ID:" + id + " Value:" + value);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message, ex);
                        }
                    }
                    break;

                case EventCurrentTargetStateTargetUpdated.MessageId:
                    {
                        try
                        {
                            var r = (EventCurrentTargetStateTargetUpdated)response;
                            var target = r.CurrentTargetStateDelta;
                            if (target != null && target.TargetState != null && target.TargetState.MediaCapacity != null)
                            {
                                var id = 0;
                                if (target.Requests.Count > 0)
                                {
                                    id = target.Requests[0].Tag;
                                }
                                if (id > 0)
                                {
                                    var mc = target.TargetState.MediaCapacity;
                                    var haschat = false;
                                    foreach (MediaCapacityInfo mcitem in mc)
                                    {
                                        if (mcitem.MediaType == "chat")
                                        {
                                            if (mcitem.CurrentMarginCount.HasValue)
                                            {
                                                haschat = true;
                                                var isready = (mcitem.CurrentMarginCount > 0);

                                                if (ALLStatisticItems.ContainsKey(id))
                                                {
                                                    var item = ALLStatisticItems[id];
                                                    item.LastDate = DateTime.Now;
                                                    item.LastValue = isready ? 4 : 8;
                                                    item.CurrentChatCount = mcitem.CurrentInteractionsCount ?? 0;
                                                    log.Debug("#EventCurrentTargetStateTargetUpdated# TypeID:" + item.TypeID + " ID:" + id + " Value:" + item.LastValue);
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    if (!haschat)
                                    {
                                        //if (ALLStatisticItems.ContainsKey(id))
                                        //{
                                        //    var item = ALLStatisticItems[id];
                                        //    item.LastDate = DateTime.Now;
                                        //    item.LastValue = 8;
                                        //}
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message, ex);
                        }
                    }
                    break;

                case EventCurrentTargetStateSnapshot.MessageId:
                    {
                        try
                        {
                            var r = (EventCurrentTargetStateSnapshot)response;
                            var id = r.Tag;
                            if (id > 0)
                            {
                                var target = r.CurrentTargetStateSnapshot;
                                if (target != null)
                                {
                                    var haschat = false;
                                    foreach (CurrentTargetStateInfo tsitem in target)
                                    {
                                        if (tsitem.MediaCapacity != null)
                                        {
                                            foreach (MediaCapacityInfo mcitem in tsitem.MediaCapacity)
                                            {
                                                if (mcitem.MediaType == "chat")
                                                {
                                                    if (mcitem.CurrentMarginCount.HasValue)
                                                    {
                                                        var isready = (mcitem.CurrentMarginCount > 0);
                                                        if (ALLStatisticItems.ContainsKey(id))
                                                        {
                                                            var item = ALLStatisticItems[id];
                                                            item.LastDate = DateTime.Now;
                                                            item.LastValue = isready ? 4 : 8;
                                                            item.CurrentChatCount = mcitem.CurrentInteractionsCount ?? 0;
                                                            log.Debug("#EventCurrentTargetStateSnapshot# TypeID:" + item.TypeID + " ID:" + id + " Value:" + item.LastValue);
                                                        }
                                                    }
                                                    haschat = true;
                                                    break;
                                                }
                                            }
                                            if (haschat)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (!haschat)
                                    {
                                        //if (ALLStatisticItems.ContainsKey(id))
                                        //{
                                        //    var item = ALLStatisticItems[id];
                                        //    if (item.LastValue == 4)
                                        //    {
                                        //        item.LastDate = DateTime.Now;
                                        //        item.LastValue = 8;
                                        //    }
                                        //}
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message, ex);
                        }
                    }
                    break;
            }
        }
        #endregion

        #region ***************************检测***************************
        /// <summary>
        /// 检测：打开状态 如果没有更新 则使用Get请求统计。
        /// </summary>
        /// <param name="stateInfo"></param>
        private static void CheckLastTime(Object stateInfo)
        {
            while (!IsEnd)
            {
                Thread.Sleep(100);
                if (IsReInit || IsEnd)
                {
                    Thread.Sleep(100);
                    continue;
                }
                if (1 == 1)
                {
                    log.Debug("[CheckLastTime]");
                    var current = DateTime.Now.AddSeconds(-10);
                    var query = ALLStatisticItems.Values.Where(x => x.REQ_ID > 0 && x.LastDate < current).ToList();
                    foreach (var item in query)
                    {
                        if (IsReInit || IsEnd)
                        {
                            Thread.Sleep(100);
                            break;
                        }
                        PeekStatistic(item);
                        Thread.Sleep(20);
                    }
                }
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// 检测：需要启动的订阅
        /// </summary>
        /// <param name="stateInfo"></param>
        private static void CheckRequireOpenStatistic(Object stateInfo)
        {
            while (!IsEnd)
            {
                Thread.Sleep(1000);
                if (IsReInit || IsEnd)
                {
                    Thread.Sleep(100);
                    continue;
                }
                log.Debug("[CheckRequireOpenStatistic]");
                try
                {
                    var query = ALLStatisticItems.Values.Where(x => x.RequireOpen && x.Opened == false && x.TypeID == 2).ToList();
                    foreach (var item in query)
                    {
                        if (IsReInit || IsEnd)
                        {
                            Thread.Sleep(100);
                            break;
                        }
                        item.LastDate = DateTime.Now;
                        if (item.Statistic.CacheKey == "agent/19")
                        {
                            OpenStatistic(item, NotificationMode.Immediate);
                        }
                        else
                        {
                            OpenStatistic(item);
                        }
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 检测协议健康状态
        /// </summary>
        /// <param name="stateInfo"></param>
        private static void CheckProtocolState(object stateInfo)
        {
            while (!IsEnd)
            {
                Thread.Sleep(1000);
                if (IsReInit || IsEnd)
                {
                    Thread.Sleep(100);
                    continue;
                }
                foreach (var s in ALLStatServer)
                {
                    var item = ALLStatisticItems.Values.FirstOrDefault(x => x.Statistic.ClientName == s.ClientName && x.REQ_ID > 0);
                    if (IsReInit || IsEnd)
                    {
                        Thread.Sleep(100);
                        break;
                    }
                    if (item != null)
                    {
                        var request = RequestGetStatisticProfile.Create(StatisticProfile.TimeRanges);
                        log.Debug(request.ToString());
                        try
                        {
                            Send(item, request);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message, ex);
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region ***************************API*****************************
        private static void GetStatistic(StatisticItem item)
        {
            try
            {
                RequestGetStatistic request = RequestGetStatistic.Create();

                //StatisticObject
                StatisticObject statisticObject =
                    StatisticObject.Create(
                            item.ObjectId,
                            (StatisticObjectType)item.Statistic.ObjectType,
                            TENANT_NAME);

                //StatisticMetric
                StatisticMetric statisticMetric = StatisticMetric.Create(item.Statistic.StatisticType);

                //statisticMetric.TimeProfile = "CollectorDefault";
                //if (!string.IsNullOrEmpty(timeRange))
                //{
                //    statisticMetric.TimeRange = timeRange;
                //}
                //if (!string.IsNullOrEmpty(filter))
                //{
                //    statisticMetric.Filter = filter;
                //}

                request.StatisticObject = statisticObject;
                request.StatisticMetric = statisticMetric;
                request.Tag = item.ReferenceId;

                log.Debug(request.ToString());
                log.Debug("#RequestGetStatistic_Succ# -> " + item.Statistic.Uri);
                Send(item, request);
            }
            catch (ProtocolException ex)
            {
                log.Debug("#RequestGetStatistic_Error# -> " + item.Statistic.Uri);
                log.Error(ex.Message, ex);
            }
        }
        private static void PeekStatistic(StatisticItem item)
        {
            try
            {
                RequestPeekStatistic request = RequestPeekStatistic.Create();
                request.StatisticId = item.REQ_ID;
                log.Debug(request.ToString());
                log.Debug("#RequestPeekStatistic_Succ# -> " + item.Statistic.Uri);
                Send(item, request);
            }
            catch (ProtocolException ex)
            {
                log.Debug("#RequestPeekStatistic_Error# -> " + item.Statistic.Uri);
                log.Error(ex.Message, ex);
            }
        }
        internal static void OpenStatistic(StatisticItem item, NotificationMode mode = NotificationMode.Periodical)
        {
            try
            {
                RequestOpenStatistic request = RequestOpenStatistic.Create();

                //StatisticObject
                StatisticObject statisticObject =
                    StatisticObject.Create(
                            item.ObjectId,
                            (StatisticObjectType)item.Statistic.ObjectType,
                            TENANT_NAME);

                //StatisticMetric
                StatisticMetric statisticMetric = StatisticMetric.Create(item.Statistic.StatisticType);

                //statisticMetric.TimeProfile = "CollectorDefault";
                //if (!string.IsNullOrEmpty(timeRange))
                //{
                //    statisticMetric.TimeRange = timeRange;
                //}
                //if (!string.IsNullOrEmpty(filter))
                //{
                //    statisticMetric.Filter = filter;
                //}

                // Notification
                Notification notification = Notification.Create();
                notification.Mode = mode; // To get Notifications after every defined interval
                if (mode == NotificationMode.Periodical)
                {
                    notification.Frequency = NOTIFICATION_FREQUENCY; //频次
                }
                request.StatisticObject = statisticObject;
                request.StatisticMetric = statisticMetric;
                request.Notification = notification;

                request.Tag = item.ReferenceId;

                log.Debug(request.ToString());
                log.Debug("#RequestOpenStatistic_Succ# -> " + item.Statistic.Uri);
                Send(item, request);
            }
            catch (ProtocolException ex)
            {
                log.Debug("#RequestOpenStatistic_Error# -> " + item.Statistic.Uri);
                log.Error(ex.Message, ex);
            }
        }
        private static void CloseStatistic(StatisticItem item)
        {
            try
            {
                RequestCloseStatistic request = RequestCloseStatistic.Create();
                if (item.REQ_ID > 0)
                {
                    request.StatisticId = item.REQ_ID;
                    log.Debug(request.ToString());
                    log.Debug("#RequestCloseStatistic_Succ# -> " + item.Statistic.Uri);
                    Send(item, request);
                }
            }
            catch (ProtocolException ex)
            {
                log.Debug("#RequestCloseStatistic_Error# -> " + item.Statistic.Uri);
                log.Error(ex.Message, ex);
            }
        }
        #endregion

        #region *****************************Open API*****************************

        //坐席每3秒获取一次数据
        public static AgentStatisticResult GetAgentStatisticResult(Person person)
        {
            /*
             * 	队列排队量（通过Person的队列进行查询：主队列、辅队列)
             * 	呼入量
             * 	呼出量
             * 	Chat量
             * 	AHT：呼入AHT（案面时长+通话时长）/呼入数量
             */
            var result = new AgentStatisticResult();
            var source = SupportClass.StatServerHelper.ALLStatisticItems.Values;
            var searchItems = new List<SearchItem>();

            //查队列
            foreach (var item in person.AgentInfo.SkillLevels)
            {
                var statistic = SupportClass.StatServerHelper.ALLStatistics.Where(x => x.CacheKey.StartsWith("skill/")).FirstOrDefault();
                if (statistic != null)
                {
                    searchItems.Add(new SearchItem()
                    {
                        ReferenceId = statistic.BaseReferenceId + item.DBID
                    });
                }
            }

            //查坐席相关
            foreach (var item in SupportClass.StatServerHelper.ALLStatistics.Where(x => x.CacheKey.StartsWith("agent/")))
            {
                searchItems.Add(new SearchItem()
                {
                    ReferenceId = item.BaseReferenceId + person.DBID
                });
            }

            var qs = (from a in source
                      join b in searchItems on a.ReferenceId equals b.ReferenceId
                      select new { a.Statistic.CacheKey, a.LastValue, a.DBID }).ToList();

            var qcount1 = 0;
            var qcount2 = 0;
            var a16 = 0;
            var a17 = 0;
            foreach (var q in qs)
            {
                if (q.CacheKey.StartsWith("skill/"))
                {
                    var qcurrent = person.AgentInfo.SkillLevels.Where(x => x.DBID == q.DBID).FirstOrDefault();
                    if (qcurrent != null)
                    {
                        if (qcurrent.Level == 9)
                        {
                            qcount1 += q.LastValue;
                        }
                        else
                        {
                            qcount2 += q.LastValue;
                        }
                    }
                }
                else if (q.CacheKey.StartsWith("agent/"))
                {
                    var no = q.CacheKey.Split('/')[1];
                    switch (no)
                    {
                        case "12":
                            result.CallInCount = q.LastValue;
                            break;
                        case "13":
                            result.CallOutCount = q.LastValue;
                            break;
                        case "14":
                            result.ChatInCount = q.LastValue;
                            break;
                        case "16":
                            a16 = q.LastValue;
                            break;
                        case "17":
                            a17 = q.LastValue;
                            break;
                    }
                }
            }

            if ((result.CallInCount + result.CallOutCount) > 0)
            {
                result.AHT = (a16 + a17) / (result.CallInCount + result.CallOutCount);
            }

            result.QueueCount1 = qcount1;
            result.QueueCount2 = qcount2;
            return result;
        }

        //通过队列名查找队列排队量
        public static int GetQueneCountBySkillName(string skillName)
        {
            var skill = ALLSkills.FirstOrDefault(x => x.Name == skillName);
            if (skill != null)
            {
                var query = ALLStatisticItems.Values.FirstOrDefault(x => x.DBID == skill.DBID && x.Statistic.StatisticType == "Current_In_Queue");

                if (query != null)
                {
                    return query.LastValue;
                }
            }
            return 0;
        }

        //查找队列对应的坐席(chat)
        public static List<TransferChatAgent> GetTransferChatAgent(string skillName)
        {
            var result = new List<TransferChatAgent>();
            var skill = ALLSkills.Where(x => x.Name == skillName).FirstOrDefault();

            if (skill != null)
            {
                var skilldbid = skill.DBID;
                var query = ALLPersons.Where(x => x.CHAT > 0 && x.AgentInfo.SkillLevels.Exists(y => y.DBID == skilldbid))
                     .Select(x => new TransferChatAgent() { AgentName = x.FirstName, UserName = x.UserName, AgentId = x.EmployeeID, PlaceId = x.Place, DBID = x.DBID }).ToList();
                var q2 = from a in ALLStatisticItems.Values
                         join b in query on a.DBID equals b.DBID
                         where a.Statistic.CacheKey == "agent/19"
                         select new TransferChatAgent() { DBID = b.DBID, AgentName = b.AgentName, UserName = b.UserName, AgentId = b.AgentId, PlaceId = b.PlaceId, IsReady = a.LastValue == 4, CurrentChatCount = a.CurrentChatCount };
                result = q2.ToList();
            }
            return result;
        }

        //批量获取工程忙闲状态，以及排队人数
        public static List<CSDP_ChatAgent> CSDP_ChatAgent_GetStatus(List<string> EmployeeIDs)
        {
            var result = new List<CSDP_ChatAgent>();

            var skillEmp = new List<CSDP_ChatAgentQuene>();
            EmployeeIDs.ForEach(x =>
            {
                skillEmp.Add(new CSDP_ChatAgentQuene() { VQ = "Q" + x + "_CSDP", AgentId = x });
            });

            var agent_skill_rel = (from a in ALLSkills
                                   join b in skillEmp on a.Name equals b.VQ
                                   select new { Skill_DBID = a.DBID, AgentId = b.AgentId }).ToList();

            //坐席chat排队量
            var agent_placeInQuene_rel = (from a in ALLStatisticItems.Values
                                          join b in agent_skill_rel on a.DBID equals b.Skill_DBID
                                          where a.Statistic.CacheKey == "skill/Switch_MM"
                                          select new { a.LastValue, b.AgentId }).ToList();


            var persons = (from x in ALLPersons
                           join b in EmployeeIDs on x.EmployeeID equals b
                           where (x.CHAT > 0)
                           select (new CSDP_ChatAgent() { DBID = x.DBID, DN = x.DN, AgentId = x.EmployeeID })
                           ).ToList();

            //查坐席状态
            var q2 = from a in ALLStatisticItems.Values
                     join b in persons on a.DBID equals b.DBID
                     join c in agent_placeInQuene_rel on b.AgentId equals c.AgentId
                     where a.Statistic.CacheKey == "agent/19"
                     select new CSDP_ChatAgent()
                     {
                         AgentId = b.AgentId,
                         IsReady = a.LastValue == 4,
                         CurrentChatCount = a.CurrentChatCount,
                         DN = b.DN,
                         Place_In_Quene = c.LastValue
                     };

            result = q2.ToList();

            result.ForEach(x =>
            {
                if (x.DN == "")
                {
                    x.Status = 3;
                }
                else
                {
                    if (x.IsReady)
                    {
                        x.Status = 1;
                    }
                    else
                    {
                        x.Status = 2;
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// 所有空闲工程师
        /// </summary>
        /// <returns></returns>
        public static List<CSDP_ChatAgentQuene_AllReady_Result> CSDP_ChatAgent_GetAllReady()
        {
            var result = new List<CSDP_ChatAgentQuene_AllReady_Result>();

            var persons = ALLPersons.Where(x => x.CHAT > 0 && x.DN != "")
                 .Select(x => new CSDP_ChatAgent() { DBID = x.DBID, DN = x.DN, AgentId = x.EmployeeID }).ToList();

            var q2 = from a in ALLStatisticItems.Values
                     join b in persons on a.DBID equals b.DBID
                     where a.Statistic.CacheKey == "agent/19" && a.LastValue == 4 && a.CurrentChatCount < 4
                     select new CSDP_ChatAgentQuene_AllReady_Result() { AgentId = b.AgentId };

            result = q2.ToList();

            return result;
        }

        //查找队列对应的坐席(voice)
        public static List<TransferAgent> GetTransferAgent(string skillName)
        {
            var result = new List<TransferAgent>();
            var skill = ALLSkills.Where(x => x.Name == skillName).FirstOrDefault();

            if (skill != null)
            {
                var skilldbid = skill.DBID;

                var query = ALLPersons.Where(x => x.VOICE > 0 && x.AgentInfo.SkillLevels.Exists(y => y.DBID == skilldbid))
                    .Select(x => new TransferAgent() { DN = x.DN, AgentName = x.FirstName, DBID = x.DBID }).ToList();

                var q2 = from a in ALLStatisticItems.Values
                         join b in query on a.DBID equals b.DBID
                         where a.Statistic.CacheKey == "agent/18"
                         select new TransferAgent() { DBID = b.DBID, AgentName = b.AgentName, DN = b.DN, IsReady = a.LastValue == 4 };
                result = q2.ToList();

            }
            return result;
        }
        internal static void CloseStatistic(int referenceId)
        {
            if (ALLStatisticItems.ContainsKey(referenceId))
            {
                var item = ALLStatisticItems[referenceId];
                CloseStatistic(item);
            }
        }
        #endregion

        #region 坐席队列变化时更改 UpdateAgentSkills
        /// <summary>
        /// 坐席队列变化时更改
        /// </summary>
        /// <param name="personDBID"></param>
        /// <param name="items"></param>
        /// <param name="type"></param>
        public static void UpdateAgentSkills(int personDBID, List<Models.PersonSkillNotification> items, UpdateAgentSkillType type)
        {
            if (items.Count == 0)
            {
                return;
            }
            var person = ALLPersons.FirstOrDefault(x => x.DBID == personDBID);
            if (person != null)
            {
                switch (type)
                {
                    case UpdateAgentSkillType.Add:
                        {
                            foreach (var item in items)
                            {
                                person.AgentInfo.SkillLevels.Add(new SkillLevel() { DBID = item.skillDBID, Level = item.level });
                            }
                        }
                        break;
                    case UpdateAgentSkillType.Update:
                        {
                            foreach (var item in items)
                            {
                                var current = person.AgentInfo.SkillLevels.FirstOrDefault(x => x.DBID == item.skillDBID);
                                if (current != null)
                                {
                                    current.Level = item.level;
                                }
                            }
                        }
                        break;
                    case UpdateAgentSkillType.Delete:
                        foreach (var item in items)
                        {
                            var current = person.AgentInfo.SkillLevels.FirstOrDefault(x => x.DBID == item.skillDBID);
                            if (current != null)
                            {
                                person.AgentInfo.SkillLevels.Remove(current);
                            }
                        }
                        break;
                }

            }
        }

        /// <summary>
        /// 新加队列要打开统计
        /// </summary>
        /// <param name="dbid"></param>
        public static void SkillCreated(int dbid, string name)
        {
            try
            {
                var skills = SupportClass.CfgServerHelper.GetSkills(dbid, name);
                if (skills.Count > 0)
                {
                    ALLSkills.Add(skills.First());
                    var items = SupportClass.StatisticXMLHelper.GetStatisticItems(null, skills);
                    foreach (var item in items)
                    {
                        ALLStatisticItems.TryAdd(item.ReferenceId, item);
                        OpenStatistic(item);
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        /// <summary>
        /// 删除的队列关闭统计
        /// </summary>
        /// <param name="dbid"></param>
        public static void SkillDeleted(int dbid)
        {
            var info = ALLSkills.FirstOrDefault(x => x.DBID == dbid);
            if (info != null)
            {
                var query = ALLStatisticItems.Values.Where(x => x.TypeID == 1 && x.DBID == dbid).ToList();
                foreach (var item in query)
                {
                    CloseStatistic(item);
                    Thread.Sleep(50);
                }
                ALLSkills.Remove(info);
            }
        }
        #endregion

    }

    /// <summary>
    /// 修改坐席队列类型
    /// </summary>
    public enum UpdateAgentSkillType
    {
        Add,
        Delete,
        Update
    }


#pragma warning restore 612, 618
}