using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.AgentManagement;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.ApplicationBlocks.Commons;

namespace Chat.AgentInterface
{
    public class OpenMediaNotReadyAction : BaseAction
    {
        public OpenMediaNotReadyAction(IProtocol protocol) : 
            base(protocol)
        {
            base.Request = RequestNotReadyForMedia.Create("chat", null);

            OrPredicate<IMessage> filter = new OrPredicate<IMessage>(
                new MessageIdFilter(EventAck.MessageId),
                new MessageIdFilter(EventAgentNotAvailable.MessageId));

            filter.AddPredicate(new AgentStatusFilter("chat", 0));

            base.SuccessFilter = filter;

            base.FailureFilter = new MessageIdFilter(EventError.MessageId);

        }
    }
}
