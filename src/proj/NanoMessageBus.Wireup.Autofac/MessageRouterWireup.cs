namespace NanoMessageBus.Wireup
{
	using Autofac;
	using Autofac.Core;
	using Core;
	using Endpoints;
	using Transports;

	public class MessageRouterWireup : WireupModule
	{
		private int maxAttempts = 3;

		public MessageRouterWireup(IWireup parent)
			: base(parent)
		{
		}

		public virtual MessageRouterWireup AttemptOnFailureAtLeast(int times)
		{
			this.maxAttempts = times;
			return this;
		}

		protected override void Load(ContainerBuilder builder)
		{
			// TODO: move more of this into virtual methods
			builder
				.Register(this.BuildRouter)
				.As<IRouteMessagesToHandlers>()
				.InstancePerDependency()
				.ExternallyOwned();

			builder
				.Register(c => new TransactionScopeUnitOfWork())
				.As<IHandleUnitOfWork>()
				.InstancePerDependency()
				.ExternallyOwned();

			builder
				.Register(c => new MessageHandlerTable<IComponentContext>(
					c, c.Resolve<IDiscoverMessageTypes>()))
				.As<ITrackMessageHandlers>()
				.InstancePerLifetimeScope()
				.ExternallyOwned();

			builder
				.Register(this.BuildMessageTypeDiscoverer)
				.As<IDiscoverMessageTypes>()
				.SingleInstance()
				.ExternallyOwned();

			builder
				.Register(c => new TransportMessageHandler(
					c.Resolve<ITrackMessageHandlers>(),
					c.Resolve<IMessageContext>()))
				.As<TransportMessageHandler>()
				.InstancePerLifetimeScope()
				.ExternallyOwned();

			builder
				.Register(this.BuildPoisonMessageHandler)
				.As<IHandlePoisonMessages>()
				.SingleInstance()
				.ExternallyOwned();

			this.Configure<MessageHandlerWireup>().AddHandler(c => c.Resolve<TransportMessageHandler>());
		}
		protected virtual IRouteMessagesToHandlers BuildRouter(IComponentContext c)
		{
			return c.Resolve<ILifetimeScope>()
				.BeginLifetimeScope(this.RegisterMessageContextWithChildLifetimeScope)
				.Resolve<MessageRouter>();
		}
		protected virtual void RegisterMessageContextWithChildLifetimeScope(ContainerBuilder builder)
		{
			builder
				.Register(c => new MessageRouter(
					c.Resolve<ILifetimeScope>(),
					c.Resolve<IHandleUnitOfWork>(),
					c.Resolve<ITransportMessages>(),
					c.Resolve<ITrackMessageHandlers>(),
					c.Resolve<IHandlePoisonMessages>()))
				.As<MessageRouter>()
				.As<IMessageContext>()
				.InstancePerLifetimeScope()
				.ExternallyOwned();
		}
		protected virtual IDiscoverMessageTypes BuildMessageTypeDiscoverer(IComponentContext c)
		{
			return new MessageTypeDiscoverer();
		}
		protected virtual IHandlePoisonMessages BuildPoisonMessageHandler(IComponentContext c)
		{
			return new PoisonMessageHandler(
				c.ResolveNamed<ISendToEndpoints>(EndpointWireup.PoisonEndpoint, new Parameter[0]),
				this.maxAttempts);
		}
	}
}