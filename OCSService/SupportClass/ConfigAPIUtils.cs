using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.Configuration.Protocols.Types;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Connection;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Configuration.Protocols;
using Genesyslab.Platform.Configuration.Protocols.ConfServer;
using Genesyslab.Platform.Configuration.Protocols.ConfServer.Events;
using Genesyslab.Platform.Configuration.Protocols.ConfServer.Requests.Objects;

using log4net;
using System.Reflection;

namespace OCSService.SupportClass
{
#pragma warning disable 612, 618
    public class ConfigAPIUtils
    {
        public static ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static IConfService InitializeConfigService(string cfgsrvHost,
                    int cfgsrvPort, string username, string password)
        {


            Endpoint confServerUri = new Endpoint(new Uri("tcp://" + cfgsrvHost + ":" + cfgsrvPort));
            String _clientName = "default";
            ConfServerProtocol cfgServiceProtocol;
            EventBrokerService _eventBrokerService;


            cfgServiceProtocol = new ConfServerProtocol(confServerUri);
            cfgServiceProtocol.ClientName = _clientName;
            cfgServiceProtocol.UserName = username;
            cfgServiceProtocol.UserPassword = password;
            cfgServiceProtocol.ClientApplicationType = (int)CfgAppType.CFGSCE;

            try
            {
                cfgServiceProtocol.Open();

            }
            catch (ProtocolException e)
            {
                log.Info(e.Message);
            }
            catch (Exception e)
            {
                log.Info(e.Message);
            }

            _eventBrokerService = BrokerServiceFactory
                    .CreateEventBroker(cfgServiceProtocol);

            IConfService cfgService = ConfServiceFactory.CreateConfService(
                    cfgServiceProtocol, _eventBrokerService);

            return cfgService;
        }

        /// <summary>
        /// 获取外拨活动list
        /// </summary>
        /// <param name="_confService"></param>
        /// <param name="campaignname"></param>
        /// <param name="tenantid"></param>
        /// <param name="switchname"></param>
        public static ICollection<CfgCampaign> RetrieveCampaignList(IConfService _confService,
            int tenantid)
        {

            ICollection<CfgCampaign> campaign = null;

            CfgCampaignQuery qcampaign = new CfgCampaignQuery(_confService);
            try
            {

                qcampaign.TenantDbid = tenantid;


                campaign = _confService
                    .RetrieveMultipleObjects<CfgCampaign>(qcampaign);
            }
            catch (Exception ex)
            {
                log.Error("获取campaign列表失败   " + ex.Message);
            }

            return campaign;
        }

        /// <summary>
        /// 获取外拨群组列表
        /// </summary>
        /// <param name="_confService"></param>
        /// <param name="campaignname"></param>
        /// <param name="tenantid"></param>
        /// <param name="switchname"></param>
        public static ICollection<CfgCampaignGroup> RetrieveCampaignGroupList(IConfService _confService, CfgCampaign campaign,
            int tenantid)
        {

            ICollection<CfgCampaignGroup> campaigngroup = null;

            CfgCampaignGroupQuery qcampaigngroup = new CfgCampaignGroupQuery(_confService);
            try
            {

                qcampaigngroup.TenantDbid = tenantid;
                qcampaigngroup.CampaignDbid = campaign.DBID;


                campaigngroup = _confService
                    .RetrieveMultipleObjects<CfgCampaignGroup>(qcampaigngroup);
            }
            catch (Exception ex)
            {
                log.Error("获取CfgCampaignGroup列表失败   " + ex.Message);
            }

            return campaigngroup;
        }

        /// <summary>
        /// 获取座席组列表
        /// </summary>
        /// <param name="_confService"></param>
        /// <param name="campaignname"></param>
        /// <param name="tenantid"></param>
        /// <param name="switchname"></param>
        public static ICollection<CfgAgentGroup> RetrieveAgentGroupList(IConfService _confService,
            int tenantid)
        {

            ICollection<CfgAgentGroup> agentgroup = null;

            CfgAgentGroupQuery qagentgroup = new CfgAgentGroupQuery(_confService);
            try
            {

                qagentgroup.TenantDbid = tenantid;


                agentgroup = _confService
                    .RetrieveMultipleObjects<CfgAgentGroup>(qagentgroup);
            }
            catch (Exception ex)
            {
                log.Error("获取qagentgroup列表失败   " + ex.Message);
            }

            return agentgroup;
        }


        /// <summary>
        /// 获取座席列表
        /// </summary>
        /// <param name="_confService"></param>
        /// <param name="campaignname"></param>
        /// <param name="tenantid"></param>
        /// <param name="switchname"></param>
        public static ICollection<CfgPerson> RetrieveAgentList(IConfService _confService,
            int tenantid)
        {

            ICollection<CfgPerson> person = null;

            CfgPersonQuery qperson = new CfgPersonQuery(_confService);
            try
            {

                qperson.TenantDbid = tenantid;


                person = _confService
                    .RetrieveMultipleObjects<CfgPerson>(qperson);
            }
            catch (Exception ex)
            {
                log.Error("获取qperson列表失败   " + ex.Message);
            }

            return person;
        }

        /// <summary>
        /// 获取座席组
        /// </summary>
        /// <param name="_confService"></param>
        /// <param name="campaignname"></param>
        /// <param name="tenantid"></param>
        /// <param name="switchname"></param>
        public static CfgAgentGroup RetrieveAgentGroup(IConfService _confService, int dbid,
            int tenantid)
        {

            CfgAgentGroup agentgroup = null;

            CfgAgentGroupQuery qagentgroup = new CfgAgentGroupQuery(_confService);
            try
            {

                qagentgroup.TenantDbid = tenantid;
                qagentgroup.Dbid = dbid;

                agentgroup = _confService
                    .RetrieveObject<CfgAgentGroup>(qagentgroup);
            }
            catch (Exception ex)
            {
                log.Error("获取qagentgroup列表失败   " + ex.Message);
            }

            return agentgroup;
        }

        public static ICollection<CfgPerson> RetrieveAgentListfromAgentGroup(IConfService _confService,
          int tenantid, CfgGroup group)
        {

            ICollection<CfgPerson> person = null;

            CfgAgentGroup agentgroup = null;


            CfgAgentGroupQuery qagentgroup = new CfgAgentGroupQuery(_confService);
            try
            {

                qagentgroup.TenantDbid = tenantid;
                qagentgroup.Dbid = group.DBID;

                agentgroup = _confService
                    .RetrieveObject<CfgAgentGroup>(qagentgroup);

                person = agentgroup.Agents;
            }
            catch (Exception ex)
            {
                log.Error("获取qagentgroup列表失败   " + ex.Message);
            }


            return person;
        }




        /// <summary>
        /// 获取DN列表
        /// </summary>
        /// <param name="_confService"></param>
        /// <param name="campaignname"></param>
        /// <param name="tenantid"></param>
        /// <param name="switchname"></param>
        public static ICollection<CfgDN> RetrieveDNList(IConfService _confService,
            CfgDNType dntype,
            int tenantid)
        {

            ICollection<CfgDN> dn = null;

            CfgDNQuery qdn = new CfgDNQuery(_confService);
            try
            {

                qdn.TenantDbid = tenantid;
                qdn.DnType = dntype;

                dn = _confService
                    .RetrieveMultipleObjects<CfgDN>(qdn);
            }
            catch (Exception ex)
            {
                log.Error("获取CfgDN列表失败   " + ex.Message);
            }

            return dn;
        }



        /// <summary>
        /// 获取CfgTableAccess列表
        /// </summary>
        /// <param name="_confService"></param>
        /// <param name="campaignname"></param>
        /// <param name="tenantid"></param>
        /// <param name="switchname"></param>
        public static ICollection<CfgTableAccess> RetrieveTableAccessList(IConfService _confService,
            int tenantid)
        {

            ICollection<CfgTableAccess> dn = null;

            CfgTableAccessQuery qdn = new CfgTableAccessQuery(_confService);
            try
            {

                qdn.TenantDbid = tenantid;
                qdn.Type = CfgTableType.CFGTTCallingList;

                dn = _confService
                    .RetrieveMultipleObjects<CfgTableAccess>(qdn);
            }
            catch (Exception ex)
            {
                log.Error("获取CfgTableAccess列表失败   " + ex.Message);
            }

            return dn;
        }


        /// <summary>
        /// 创建外拨活动
        /// </summary>
        /// <param name="_confService"></param>
        /// <param name="campaignname"></param>
        /// <param name="tenantid"></param>
        /// <param name="switchname"></param>
        public static void CreateCampaign(IConfService _confService, string campaignname,
            int tenantid, string switchname)
        {

            CfgTenantQuery qTenant = new CfgTenantQuery();
            qTenant.Dbid = tenantid;
            CfgTenant tenant = _confService.RetrieveObject<CfgTenant>(qTenant);
            CfgSwitchQuery qSwitch = new CfgSwitchQuery();
            qSwitch.Name = switchname;
            qSwitch.TenantDbid = tenant.DBID;
            CfgSwitch @switch = _confService
            .RetrieveObject<CfgSwitch>(qSwitch);

            CfgCampaign campaign = new CfgCampaign(_confService);
            try
            {

                campaign.Name = campaignname;
                campaign.Tenant = tenant;


                campaign.Save();
            }
            catch (Exception ex)
            {
                log.Error("can not create campaignname " + campaignname + ":" + ex.Message);
            }

        }

        /// <summary>
        /// 删除一个外拨活动
        /// </summary>
        /// <param name="_confService"></param>
        /// <param name="campaignname"></param>
        /// <param name="tenantid"></param>
        /// <param name="switchname"></param>
        public static void DeleteCampaign(IConfService _confService, string campaignname,
         int tenantid, string switchname)
        {

            CfgCampaignQuery qcampaign = new CfgCampaignQuery(_confService);
            try
            {

                qcampaign.Name = campaignname;
                qcampaign.TenantDbid = tenantid;

                CfgCampaign campaign = _confService.RetrieveObject<CfgCampaign>(qcampaign);
                campaign.Delete();

            }
            catch (Exception ex)
            {
                log.Error("can not create campaignname " + campaignname + ":" + ex.Message);
            }

        }




        /// <summary>
        /// 获取Tenant
        /// </summary>
        /// <param name="_confService"></param>
        /// <param name="campaignname"></param>
        /// <param name="tenantid"></param>
        /// <param name="switchname"></param>
        public static CfgTenant RetrieveTenant(IConfService _confService,
            int tenantid)
        {

            CfgTenant tenant = null;

            CfgTenantQuery qtenant = new CfgTenantQuery(_confService);
            try
            {
                qtenant.Dbid = tenantid;

                tenant = _confService
                    .RetrieveObject<CfgTenant>(qtenant);
            }
            catch (Exception ex)
            {
                log.Error("获取tenant列表失败   " + ex.Message);
            }

            return tenant;
        }




        public static void FinalizeConfigService(IConfService _confService)
        {
            ConfServiceFactory.ReleaseConfService(_confService);
        }

    }
#pragma warning restore 612, 618

}
