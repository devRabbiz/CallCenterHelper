using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.ApplicationBlocks.Commons;
using Genesyslab.Platform.ApplicationBlocks.Commons.Broker;

namespace Chat.AgentInterface
{
    public class BaseAction : ISubscriber<IMessage>
    {
        #region Constructors
        
        public delegate void ActionExecutedDelegate(BaseAction action);
        public delegate void ActionFailedDelegate(BaseAction action);

        public event ActionExecutedDelegate ActionExecuted;
        public event ActionFailedDelegate ActionFailed;

        public BaseAction(IProtocol protocol)
        {
            this.Protocol = protocol;
        }
        #endregion

        #region Properties
        
        public IProtocol Protocol { get; private set; }

        public IMessage Request { get; set; }

        public IMessage Event { get; set; }

        public IPredicate<IMessage> SuccessFilter { get; set; }

        public IPredicate<IMessage> FailureFilter { get; set; }


        #endregion

        #region ISubscriber<IMessage> Members

        public IPredicate<IMessage> Filter
        {
            get
            {
                if (SuccessFilter != null && FailureFilter != null)
                {
                    return new OrPredicate<IMessage>(SuccessFilter, FailureFilter);
                }
                else if (SuccessFilter != null)
                {
                    return SuccessFilter;
                }
                else if (FailureFilter != null)
                {
                    return FailureFilter;
                }

                return null;
            }
        }

        public void Handle(IMessage obj)
        {
            if (SuccessFilter == null || obj == null) return;

            Event = obj;

            if (SuccessFilter.Invoke(obj))
            {
                HandleSuccess(obj);
            }
            else
            {
                HandleFailure(obj);
            }

        }

        private void HandleFailure(IMessage obj)
        {
            ActionFailed.Invoke(this);
        }

        private void HandleSuccess(IMessage obj)
        {
            ActionExecuted.Invoke(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 执行动作
        /// </summary>
        public virtual void Execute()
        {
            if (Request == null) throw new InvalidOperationException("This action only handles events");
            IMessage msg = this.Protocol.Request(Request);
            Handle(msg);
        }

        protected void CloneSubscription(ref BaseAction action)
        {
            action.ActionExecuted = this.ActionExecuted;
            action.ActionFailed = this.ActionFailed;
        }
        #endregion


    }
}
