namespace NanoMessageBus.Wireup
{
	using System;
	using System.Collections.Generic;
	using Autofac;
	using Endpoints;
	using MessageSubscriber;
	using SubscriptionStorage;
	using Transports;

	public class MessageSubscriberWireup : WireupModule
	{
		private readonly IDictionary<string, ICollection<Type>> requests =
			new Dictionary<string, ICollection<Type>>();

		public MessageSubscriberWireup(IWireup parent)
			: base(parent)
		{
		}

		public virtual MessageSubscriberWireup AddSubscription(string publisher, params Type[] messageTypes)
		{
			// TODO: once application is initialized, we need to dispatch these requests
			// TODO: subscription expirations;
			ICollection<Type> types;
			if (!this.requests.TryGetValue(publisher, out types))
				this.requests[publisher] = types = new HashSet<Type>();

			foreach (var messageType in messageTypes)
				types.Add(messageType);

			return this;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder
				.Register(this.BuildMessageSubsciber)
				.As<ISubscribeToMessages>()
				.As<IUnsubscribeFromMessages>()
				.SingleInstance()
				.ExternallyOwned();

			builder
				.Register(this.BuildMessageHandlers)
				.As<SubscriptionMessageHandlers>()
				.InstancePerDependency() // this MessageContext only exists in child container, so we can't do SingleInstance
				.ExternallyOwned();

			this.RegisterMessageHandlerRoutes();
		}
		protected virtual MessageSubscriber BuildMessageSubsciber(IComponentContext c)
		{
			return new MessageSubscriber(
				c.Resolve<IReceiveFromEndpoints>().EndpointAddress,
				c.Resolve<ITransportMessages>());
		}
		protected virtual SubscriptionMessageHandlers BuildMessageHandlers(IComponentContext c)
		{
			return new SubscriptionMessageHandlers(
				c.Resolve<IStoreSubscriptions>(),
				c.Resolve<IMessageContext>());
		}
		protected virtual void RegisterMessageHandlerRoutes()
		{
			this.Configure<MessageHandlerWireup>().AddHandler<SubscriptionRequestMessage>(
				c => c.Resolve<SubscriptionMessageHandlers>());
			this.Configure<MessageHandlerWireup>().AddHandler<UnsubscribeRequestMessage>(
				c => c.Resolve<SubscriptionMessageHandlers>());
		}
	}
}