using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Genesyslab.Platform.WebMedia.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Chat.Common;

namespace Chat.AgentInterface
{
    public class ChatJoinAction : BaseAction
    {
        public ChatJoinAction(IProtocol protocol)
            : base(protocol)
        {
            base.SuccessFilter = new ChatJoinFilter();
            base.FailureFilter = new MessageIdFilter(EventError.MessageId);
        }

        public void Execute(string interactionId, string messageText)
        {
            try
            {
                RequestJoin reqJoin = RequestJoin.Create(interactionId, Visibility.All, MessageText.Create(messageText));
                BaseAction action = new ChatJoinAction(Protocol);
                action.Request = reqJoin;
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
