using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.ApplicationBlocks.Commons;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;

namespace Chat.AgentInterface
{
    public class ChatNoticeFilter : IPredicate<IMessage>
    {
        #region IPredicate<IMessage> Members

        public bool Invoke(IMessage obj)
        {
            EventSessionInfo sessionInfo = obj as EventSessionInfo;
            bool br = false;

            if (sessionInfo == null) return false;

            NoticeInfo noticeInfo =
                sessionInfo.ChatTranscript.ChatEventList.GetAsNoticeInfo(
                    sessionInfo.ChatTranscript.ChatEventList.Count - 1);

            if (noticeInfo != null)
            {
                br = true;
            }

            return br;
        }

        #endregion
    }
}
