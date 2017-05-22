using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.WebMedia.Protocols;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Chat.Common;

namespace Chat.AgentInterface
{
    public class ChatMessageAction : BaseAction
    {
        public ChatMessageAction(IProtocol protocol)
            : base(protocol)
        {
            base.SuccessFilter = new ChatMessageFilter();
            base.FailureFilter = new MessageIdFilter(EventError.MessageId);
        }

        public void Execute(string interactionId, string messageText)
        {
            try
            {
                RequestMessage reqMessage = RequestMessage.Create(
                    interactionId,
                    Genesyslab.Platform.WebMedia.Protocols.BasicChat.Visibility.All,
                    MessageText.Create(messageText));

                BaseAction action = new ChatMessageAction(Protocol);

                action.Request = reqMessage;

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
