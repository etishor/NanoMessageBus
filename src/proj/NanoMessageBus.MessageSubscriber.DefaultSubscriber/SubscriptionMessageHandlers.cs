namespace NanoMessageBus.MessageSubscriber
{
	using System;
	using SubscriptionStorage;

	public class SubscriptionMessageHandlers : IHandleMessages<SubscriptionRequestMessage>,
		IHandleMessages<UnsubscribeRequestMessage>
	{
		private readonly IStoreSubscriptions storage;
		private readonly Uri subscriberAddress;

		public SubscriptionMessageHandlers(IStoreSubscriptions storage, IMessageContext context)
		{
			this.storage = storage;
			this.subscriberAddress = context.CurrentMessage.ReturnAddress;
		}

		public void Handle(SubscriptionRequestMessage message)
		{
			this.storage.Subscribe(this.subscriberAddress, message.MessageTypes, message.Expiration);
		}
		public void Handle(UnsubscribeRequestMessage message)
		{
			this.storage.Unsubscribe(this.subscriberAddress, message.MessageTypes);
		}
	}
}