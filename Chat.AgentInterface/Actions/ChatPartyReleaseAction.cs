using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Chat.Common;

namespace Chat.AgentInterface
{
    public class ChatPartyReleaseAction : BaseAction
    {
        public ChatPartyReleaseAction(IProtocol protocol)
            : base(protocol)
        {
            base.SuccessFilter = new ChatPartyLeftFilter();
            base.FailureFilter = new MessageIdFilter(EventError.MessageId);
        }

        public void Execute(string sessionId, string userId
            , Genesyslab.Platform.WebMedia.Protocols.BasicChat.Action afterAction)
        {
            try
            {
                RequestReleaseParty reqReleaseParty = RequestReleaseParty.Create(
                    sessionId, userId, afterAction, null);

                BaseAction action = new ChatPartyReleaseAction(Protocol);

                action.Request = reqReleaseParty;

                base.CloneSubscription(ref action);

                action.Execute();
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
        }

    }
}
