using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;
using Genesyslab.Platform.ApplicationBlocks.Commons;
using Genesyslab.Platform.Commons.Collections;

namespace Chat.AgentInterface
{
    public class OpenMediaLoginAction : BaseAction
    {
        public OpenMediaLoginAction(IProtocol protocol, int tenantId, string placeId, string agentId) : 
            base(protocol)
        {
            RequestAgentLogin reqAgentLogin = RequestAgentLogin.Create(tenantId, placeId, null);

            reqAgentLogin.AgentId = agentId;
            reqAgentLogin.MediaList = new KeyValueCollection(); ;
            reqAgentLogin.MediaList.Add("chat",1);
                
            base.Request = reqAgentLogin;

            base.SuccessFilter = new OrPredicate<IMessage>(
                new MessageIdFilter(EventAck.MessageId),
                new MessageIdFilter(EventAgentLogin.MessageId));

            base.FailureFilter = new MessageIdFilter(EventError.MessageId);
        }

    }
}
