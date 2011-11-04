namespace NanoMessageBus
{
    using System.Collections.Generic;
    using Core;

    public class TransactionalBus : ISendMessages, IPublishMessages
    {
        private readonly IHandleUnitOfWork unitOfWork;
        private readonly MessageBus inner;

        public TransactionalBus(IHandleUnitOfWork unitOfWork, MessageBus inner)
        {
            this.unitOfWork = unitOfWork;
            this.inner = inner;
        }

        public virtual void Send(params object[] messages)
        {
            this.unitOfWork.Register(() => this.inner.Send(messages));
        }

        public virtual void Reply(params object[] messages)
        {
            this.unitOfWork.Register(() => this.inner.Reply(messages));
        }

        public virtual void Publish(params object[] messages)
        {
            this.unitOfWork.Register(() => this.inner.Publish(messages));
        }
    }
}