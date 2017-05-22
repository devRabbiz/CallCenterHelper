using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionDelivery;
using Genesyslab.Platform.ApplicationBlocks.Commons;

namespace Chat.AgentInterface
{
    public class OpenMediaRejectAction : BaseAction
    {
        public OpenMediaRejectAction(IProtocol protocol)
            : base(protocol)
        {
            base.SuccessFilter = new OrPredicate<IMessage>(
                new MessageIdFilter(EventRejected.MessageId),
                new MessageIdFilter(EventAck.MessageId));

            base.FailureFilter = new MessageIdFilter(EventError.MessageId);
        }

        internal void Execute(int ticketId, string interactionId)
        {
            RequestReject requestReject =
                RequestReject.Create(
                ticketId,
                interactionId,
                null);

            BaseAction action = new OpenMediaRejectAction(Protocol);

            action.Request = requestReject;

            base.CloneSubscription(ref action);

            action.Execute();

        }
    }
}
