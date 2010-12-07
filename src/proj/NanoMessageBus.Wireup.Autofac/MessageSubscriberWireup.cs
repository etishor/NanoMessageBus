namespace NanoMessageBus.Wireup
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Autofac;
	using Autofac.Core;
	using Endpoints;
	using MessageSubscriber;
	using SubscriptionStorage;
	using Transports;

	public class MessageSubscriberWireup : WireupModule
	{
		private readonly IDictionary<Uri, ICollection<Type>> requests =
			new Dictionary<Uri, ICollection<Type>>();

		public MessageSubscriberWireup(IWireup parent)
			: base(parent)
		{
		}

		public virtual MessageSubscriberWireup AddSubscription(string publisher, params Type[] messageTypes)
		{
			// TODO: subscription expiration
			ICollection<Type> types;

			var key = new Uri(publisher);
			if (!this.requests.TryGetValue(key, out types))
				this.requests[key] = types = new HashSet<Type>();

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

			if (this.requests.Count > 0)
				builder.RegisterCallback(this.OnContainerConfigured);
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

		protected virtual void OnContainerConfigured(IComponentRegistry registry)
		{
			var service = new TypedService(typeof(ITransportMessages));
			registry.RegistrationsFor(service).First().Activated += (s, e) =>
			{
				var subscriber = e.Context.Resolve<ISubscribeToMessages>();
				foreach (var request in this.requests)
					subscriber.Subscribe(request.Key, DateTime.MaxValue, request.Value.ToArray());
			};
		}
	}
}