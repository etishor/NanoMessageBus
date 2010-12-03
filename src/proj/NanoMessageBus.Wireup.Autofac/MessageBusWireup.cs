namespace NanoMessageBus.Wireup
{
	using System;
	using System.Collections.Generic;
	using Autofac;
	using Core;
	using Endpoints;
	using SubscriptionStorage;
	using Transports;

	public class MessageBusWireup : WireupModule
	{
		private readonly IDictionary<Type, ICollection<string>> endpoints =
			new Dictionary<Type, ICollection<string>>();
		private readonly IDictionary<Type, TimeSpan> timeToLiveTypes = new Dictionary<Type, TimeSpan>();
		private readonly ICollection<Type> transientTypes = new HashSet<Type>();

		public MessageBusWireup(IWireup parent)
			: base(parent)
		{
		}

		public virtual MessageBusWireup RegisterMessageEndpoint(string endpointAddress, params Type[] messageTypes)
		{
			foreach (var messageType in messageTypes ?? new Type[] { })
			{
				ICollection<string> registered;
				if (!this.endpoints.TryGetValue(messageType, out registered))
					this.endpoints[messageType] = registered = new HashSet<string>();

				registered.Add(endpointAddress);
			}

			return this;
		}
		public virtual MessageBusWireup RegisterMessageTimeToLive(TimeSpan timeToLive, params Type[] messageTypes)
		{
			foreach (var messageType in messageTypes)
				this.timeToLiveTypes[messageType] = timeToLive;

			return this;
		}
		public virtual MessageBusWireup RegisterTransientMessage(params Type[] messageTypes)
		{
			foreach (var messageType in messageTypes)
				this.transientTypes.Add(messageType);
			
			return this;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder
				.Register(this.BuildMessageBus)
				.As<ISendMessages>()
				.As<IPublishMessages>()
				.InstancePerDependency()
				.ExternallyOwned();

			builder
				.Register(this.BuildTransportMessageBuilder)
				.As<MessageBuilder>()
				.SingleInstance()
				.ExternallyOwned();
		}
		protected virtual MessageBus BuildMessageBus(IComponentContext c)
		{
			return new MessageBus(
				c.Resolve<ITransportMessages>(),
				c.Resolve<IStoreSubscriptions>(),
				this.endpoints,
				c.ResolveOptional<IMessageContext>() ?? new NullMessageContext(),
				c.Resolve<MessageBuilder>(),
				c.Resolve<IDiscoverMessageTypes>());
		}
		protected virtual MessageBuilder BuildTransportMessageBuilder(IComponentContext c)
		{
			return new MessageBuilder(
				this.timeToLiveTypes, this.transientTypes, c.Resolve<IReceiveFromEndpoints>().EndpointAddress);
		}
	}
}