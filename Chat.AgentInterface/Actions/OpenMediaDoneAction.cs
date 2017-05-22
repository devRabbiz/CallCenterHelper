using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Requests.InteractionManagement;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.ApplicationBlocks.Commons;

namespace Chat.AgentInterface
{
    public class OpenMediaDoneAction : BaseAction
    {
        public OpenMediaDoneAction(IProtocol protocol) : base (protocol)
        {
            base.SuccessFilter = new OrPredicate<IMessage>(
                new MessageIdFilter(EventAck.MessageId), new MessageIdFilter(EventProcessingStopped.MessageId) );

            base.FailureFilter = new MessageIdFilter(EventError.MessageId);
        }

        public void Execute(string interactionId)
        {
            RequestStopProcessing requestStop =
                RequestStopProcessing.Create(
                interactionId,
                null);

            BaseAction action = new OpenMediaDoneAction(Protocol);

            action.Request = requestStop;

            base.CloneSubscription(ref action);

            action.Execute();
        }

    }
}
