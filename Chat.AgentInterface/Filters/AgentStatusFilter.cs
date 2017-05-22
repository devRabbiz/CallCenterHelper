using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.ApplicationBlocks.Commons;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;

namespace Chat.AgentInterface
{
    class AgentStatusFilter : IPredicate<IMessage>
    {
        string media;
        int status;

        public AgentStatusFilter(string media, int status)
        {
            this.media = media;
            this.status = status;
        }

        #region IPredicate<IMessage> Members

        public bool Invoke(IMessage obj)
        {
            bool br = false;

            if (obj.Id == EventCurrentAgentStatus.MessageId)
            {
                EventCurrentAgentStatus agentStatus = obj as EventCurrentAgentStatus;

                if (agentStatus.MediaStateList[media].Equals(status))
                {
                    br = true;
                }
            }

            return br;
        }

        #endregion
    }
}
