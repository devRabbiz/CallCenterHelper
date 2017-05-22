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
    public class OpenMediaLogoutAction : BaseAction
    {
        public OpenMediaLogoutAction(IProtocol protocol)
            : base(protocol)
        {
            base.Request = RequestAgentLogout.Create();
            base.SuccessFilter = new OrPredicate<IMessage>(
                new MessageIdFilter(EventAck.MessageId),
                new MessageIdFilter(EventAgentLogout.MessageId));

            base.FailureFilter = new MessageIdFilter(EventError.MessageId);
 
        }
    }
}
