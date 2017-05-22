using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.ApplicationBlocks.Commons;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;

namespace Chat.AgentInterface
{
    public class OpenMediaReadyAction : BaseAction
    {
        public OpenMediaReadyAction(IProtocol protocol) : 
            base(protocol)
        {
            base.Request = RequestCancelNotReadyForMedia.Create("chat", null);      
  
            OrPredicate<IMessage> filter = new OrPredicate<IMessage>(
                    new MessageIdFilter(EventAck.MessageId),
                    new MessageIdFilter(EventAgentAvailable.MessageId));

            filter.AddPredicate( new AgentStatusFilter("chat", 1) ) ;

            base.SuccessFilter = filter;
            base.FailureFilter = new MessageIdFilter(EventError.MessageId);
        }
    }
}
