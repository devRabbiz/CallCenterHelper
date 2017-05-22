using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Outbound.Protocols.OutboundServer;
using Genesyslab.Platform.Outbound.Protocols.OutboundServer.Events;
using Genesyslab.Platform.Outbound.Protocols.OutboundServer.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OCSService.SupportClass
{

#pragma warning disable 612, 618
    public class OCSHelper
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(OCSHelper));

        private delegate void GenesysEventHandlerDelegate(IMessage message);
        private IConfService confservice;
        private const String OCS_IDENTIFIER = "localhost_OCS";


        private static object LockObject = new Object();

        private static object LockObject1 = new Object();

        // URI to connect to Server
        private String OCS_URI = "tcp://10.0.1.102:6127";
        private String CLIENT_NAME = "OCManager";

        private ProtocolManagementService protocolManagementService;
        private OutboundServerConfiguration outboundServerConfiguration;
        private EventBrokerService eventBrokerService;

        private bool IsStart;

        public List<CampaignInfo> ALLCampaign = new List<CampaignInfo>();

        public void Start()
        {
            IsStart = true;
            try
            {
                confservice = ConfigAPIUtils.InitializeConfigService(Utils.cfgserverhost.ToString(), Utils.cfgserverport, Utils.username.ToString(), Utils.password.ToString());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            try
            {
                OCS_URI = System.Configuration.ConfigurationManager.AppSettings["OCS_URI"];
                InitializePSDKApplicationBlocks();
                IsStart = true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            Connect();
        }

        public void End()
        {
            IsStart = false;
            try
            {
                FinalizePSDKApplicationBlocks();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            FinalizePSDKApplicationBlocks();
        }

        #region Genesys Event Handlers
        private void OutboundServerEventsHandler(IMessage response)
        {
            switch (response.Id)
            {
                case EventCampaignLoaded.MessageId:
                    {
                        var r = (EventCampaignLoaded)response;
                    }
                    break;

                case EventDialingStarted.MessageId:
                    {
                        var r = (EventDialingStarted)response;
                    }
                    break;

                case EventDialingStopped.MessageId:
                    {
                        var r = (EventCampaignStatus)response;
                    }
                    break;

                case EventCampaignStatus.MessageId:
                    {
                        var r = (EventCampaignStatus)response;
                    }
                    break;

                case EventError.MessageId:
                    {
                        var r = (EventError)response;
                    }
                    break;
            }

            log.Info(response.ToString());
        }

        public void OnProtocolOpened(object sender, ProtocolEventArgs e)
        {
            log.Info("OnProtocolOpened");
        }
        public void OnProtocolClosed(object sender, ProtocolEventArgs e)
        {
            log.Info("OnProtocolClosed");
        }

        #endregion

        #region InitializePSDKApplicationBlocks  FinalizePSDKApplicationBlocks
        private void InitializePSDKApplicationBlocks()
        {

            // Setup Application Blocks:

            // Create Protocol Manager Service object
            protocolManagementService = new ProtocolManagementService();

            // Create and initialize connection configuration object for TServer.
            outboundServerConfiguration = new OutboundServerConfiguration(OCS_IDENTIFIER);

            // Set required values
            try
            {
                outboundServerConfiguration.Uri = new Uri(OCS_URI);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + "\n" + ex.StackTrace + "\n");
            }

            outboundServerConfiguration.ClientName = CLIENT_NAME;

            // Register this connection configuration with Protocol Manager
            protocolManagementService.Register(outboundServerConfiguration);

            protocolManagementService.ProtocolOpened += new EventHandler<ProtocolEventArgs>(this.OnProtocolOpened);
            protocolManagementService.ProtocolClosed += new EventHandler<ProtocolEventArgs>(this.OnProtocolClosed);


            // Create and Initialize Message Broker Application Block
            eventBrokerService = new EventBrokerService(protocolManagementService.Receiver);

            eventBrokerService.Register(
                OutboundServerEventsHandler,
                new MessageFilter(protocolManagementService[OCS_IDENTIFIER].ProtocolId));

            // Activate event broker service 
            eventBrokerService.Activate();
        }

        private void FinalizePSDKApplicationBlocks()
        {

            // Cleanup code
            eventBrokerService.Deactivate();

            eventBrokerService.Dispose();

            // Close Connection if opened (check status of protocol object)
            IProtocol protocol = protocolManagementService[OCS_IDENTIFIER];

            if (protocol.State == ChannelState.Opened) // Close only if the protocol state is opened
            {
                try
                {
                    protocol.Close();
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message + "\n" + ex.StackTrace + "\n");
                }
            }

            protocolManagementService.Unregister(OCS_IDENTIFIER);
        }
        #endregion

        #region OpenAPI
        public void Connect()
        {
            // Open the connection - only when the connection is not already opened
            // Opening the connection can fail and raises an exception
            try
            {
                IProtocol protocol = protocolManagementService[OCS_IDENTIFIER];

                if (protocol.State == ChannelState.Closed)
                    protocol.Open(); // attempt to open the channel asynchronously

            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }
        public void Disconnect()
        {
            try
            {
                // Get the protocol associated with this configuration
                IProtocol protocol = protocolManagementService[OCS_IDENTIFIER];

                // Close if protocol not already closed
                if (protocol.State == ChannelState.Opened)
                {
                    // Close the connection asynchronously
                    protocol.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        #region LoadCampaign  UnloadCampaign  StartDialing  StopDailing
        public bool LoadCampaign(int campaignId, int groupId, int sessionId, ref string errmsg)
        {
            try
            {
                RequestLoadCampaign request = RequestLoadCampaign.Create(
                        campaignId,
                        groupId,
                        sessionId,
                        null);

                log.Info(request.ToString());
                var r = GetProtocol().Request(request);
                log.Debug(r.ToString());
                var succ = r as EventCampaignLoaded;
                if (succ == null)
                {
                    errmsg = (r as EventError).ToString();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                log.Error(ex.Message, ex);
                return false;
            }
        }

        public bool UnloadCampaign(int campaignId, int groupId, int sessionId, ref string errmsg)
        {
            try
            {
                // Create the register address request object for TServer (Voice Platform SDK). 
                RequestUnloadCampaign request = RequestUnloadCampaign.Create(
                        campaignId,
                        groupId,
                        sessionId,
                        null);

                log.Debug(request.ToString());

                var r = GetProtocol().Request(request);
                log.Debug(r.ToString());
                var succ = r as EventCampaignUnloaded;
                if (succ == null)
                {
                    errmsg = (r as EventError).ToString();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                log.Error(ex.Message, ex);
                return false;
            }
        }

        public bool StartDialing(int campaignId, int groupId, int sessionId, ref string errmsg)
        {
            try
            {
                // Create the register address request object for TServer (Voice Platform SDK). 
                RequestStartDialing request = RequestStartDialing.Create();

                request.CampaignId = campaignId;
                request.GroupId = groupId;
                request.SessionId = sessionId;

                request.DialMode = DialMode.Progress;
                request.OptimizeBy = OptimizationMethod.BusyFactor;
                request.OptimizeGoal = 80;
                request.RequestProperties = null;

                log.Debug(request.ToString());

                // Request Register Address synchronously
                var r = GetProtocol().Request(request);
                log.Debug(r.ToString());
                var succ = r as EventDialingStarted;
                if (succ == null)
                {
                    errmsg = (r as EventError).ToString();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                log.Error(ex.Message, ex);
                return false;
            }
        }

        public bool StopDailing(int campaignId, int groupId, int sessionId, ref string errmsg)
        {
            try
            {
                // Create the register address request object for TServer (Voice Platform SDK). 
                RequestStopDialing request = RequestStopDialing.Create();

                request.CampaignId = campaignId;
                request.GroupId = groupId;
                request.SessionId = sessionId;

                request.RequestProperties = null;

                log.Debug(request.ToString());

                // Request Register Address synchronously
                var r = GetProtocol().Request(request);
                log.Debug(r.ToString());
                var succ = r as EventDialingStopped;
                if (succ == null)
                {
                    errmsg = (r as EventError).ToString();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                log.Error(ex.Message, ex);
                return false;
            }
        }
        #endregion

        public string GetCampaignStatus(int campaignId, int groupId, int sessionId, ref string errmsg)
        {
            try
            {
                // Create the register address request object for TServer (Voice Platform SDK). 
                RequestGetCampaignStatus request = RequestGetCampaignStatus.Create();

                if (campaignId > 0)
                {
                    request.CampaignId = campaignId;
                }
                if (groupId > 0)
                {
                    request.GroupId = groupId;
                }
                if (sessionId > 0)
                {
                    request.SessionId = sessionId;
                }

                request.RequestProperties = null;

                log.Debug(request.ToString());

                // Request Register Address synchronously
                var r = GetProtocol().Request(request);
                log.Debug(r.ToString());
                var succ = r as EventCampaignStatus;
                if (succ == null)
                {
                    errmsg = (r as EventError).ToString();
                    return string.Empty;
                }
                else
                {
                    return succ.GroupCampaignStatus.ToString();
                }
            }

            catch (Exception ex)
            {
                errmsg = ex.Message;
                log.Error(ex.Message, ex);
            }
            return string.Empty;
        }
        #endregion

        #region cfg API
        public void ReloadCampaginlist()
        {
            ALLCampaign.Clear();
            ICollection<CfgCampaign> list = ConfigAPIUtils.RetrieveCampaignList(confservice, Utils.tenantid);
            foreach (CfgCampaign c in list)
            {
                var info = new CampaignInfo();
                info.CampaignId = c.DBID;
                info.Name = c.Name;
                var groupid = 0;
                if (c.CampaignGroups.Count > 0)
                {
                    groupid = c.CampaignGroups.First().GroupDBID;
                }
                var errMsg = string.Empty;
                var r = GetCampaignStatus(info.CampaignId, groupid, 0, ref errMsg);
                if (r != "")
                {
                    info.GroupCampaignStatus = r;
                }
                ALLCampaign.Add(info);
            }
        }
        private IProtocol GetProtocol()
        {
            lock (LockObject)
            {
                var p = protocolManagementService[OCS_IDENTIFIER];
                if (IsStart)
                {
                    if (p.State == ChannelState.Closed)
                    {
                        p.Open();
                    }
                }
                return p;
            }
        }
        #endregion
    }

    public class OCSHelperInterface
    {
        private static OCSHelper _helper;

        public static OCSHelper GetHelper()
        {
            if (_helper == null)
            {
                _helper = new OCSHelper();
            }
            return _helper;
        }

    }


#pragma warning restore 612, 618
}