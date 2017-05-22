using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Events;
using Genesyslab.Platform.WebMedia.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat.Requests;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using Chat.Common;

namespace Chat.AgentInterface
{
    public class ChatNoticeAction : BaseAction
    {
        public ChatNoticeAction(IProtocol protocol)
            : base(protocol)
        {
            base.SuccessFilter = new ChatNoticeFilter();
            base.FailureFilter = new MessageIdFilter(EventError.MessageId);
        }

        public void Execute(string interactionId, NoticeType noticeType, string notice)
        {
            try
            {
                RequestNotify reqNotify = RequestNotify.Create(
                    interactionId, Visibility.All,
                    NoticeText.Create(noticeType, notice), null, null);

                BaseAction action = new ChatNoticeAction(Protocol);

                action.Request = reqNotify;

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
