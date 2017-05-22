using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.ApplicationBlocks.Commons;
using Genesyslab.Platform.Commons.Protocols;

namespace Chat.AgentInterface
{
    public class OpenMediaAcceptAction : BaseAction 
    {
        public OpenMediaAcceptAction(IProtocol protocol) : base (protocol)
        {
            base.SuccessFilter = new OrPredicate<IMessage>(
                new MessageIdFilter(EventAck.MessageId),
                new MessageIdFilter(EventAccepted.MessageId));

            base.FailureFilter = new MessageIdFilter(EventError.MessageId);
        }

        public void Execute(int ticketId, string interactionId)
        {
            RequestAccept requestAccept =
                RequestAccept.Create(
                ticketId,
                interactionId);

            BaseAction action = new OpenMediaAcceptAction(Protocol);

            action.Request = requestAccept;

            base.CloneSubscription( ref action );

            action.Execute();
        }

    }
}
