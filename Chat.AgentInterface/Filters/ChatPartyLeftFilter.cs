using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.ApplicationBlocks.Commons;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Chat.Common;

namespace Chat.AgentInterface
{
    public class ChatPartyLeftFilter : IPredicate<IMessage>
    {
        #region IPredicate<IMessage> Members

        public bool Invoke(IMessage obj)
        {
            EventSessionInfo sessionInfo = obj as EventSessionInfo;
            bool br = false;

            if (sessionInfo == null) return false;
            try
            {

                PartyLeftInfo partyLeftInfo =
                    sessionInfo.ChatTranscript.ChatEventList.GetAsPartyLeftInfo(
                        sessionInfo.ChatTranscript.ChatEventList.Count - 1);

                if (partyLeftInfo != null)
                {
                    br = true;
                }
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return br;
        }

        #endregion
    }
}
