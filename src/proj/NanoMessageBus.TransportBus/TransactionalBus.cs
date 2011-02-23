namespace NanoMessageBus
{
	using Core;

	public class TransactionalBus : ISendMessages, IPublishMessages
	{
		private readonly IManageCurrentUnitOfWork uowManager;
		private readonly MessageBus inner;

        public TransactionalBus(IManageCurrentUnitOfWork uowManager, MessageBus inner)
		{
            this.uowManager = uowManager;
			this.inner = inner;
		}

		public virtual void Send(params object[] messages)
		{
			this.uowManager.CurrentUnitOfWork.Register(() => this.inner.Send(messages));
		}
		public virtual void Reply(params object[] messages)
		{
            this.uowManager.CurrentUnitOfWork.Register(() => this.inner.Reply(messages));
		}
		public virtual void Publish(params object[] messages)
		{
            this.uowManager.CurrentUnitOfWork.Register(() => this.inner.Publish(messages));
		}
	}
}