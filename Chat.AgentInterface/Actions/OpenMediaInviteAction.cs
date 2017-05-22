using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.ApplicationBlocks.Commons;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.Commons.Protocols;

namespace Chat.AgentInterface
{
    public class OpenMediaInviteAction : BaseAction
    {
        public OpenMediaInviteAction(IProtocol protocol) : base(protocol)
        {
            base.SuccessFilter = new MessageIdFilter(EventInvite.MessageId);
        }
    }
}
