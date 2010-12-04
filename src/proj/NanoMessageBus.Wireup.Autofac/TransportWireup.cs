namespace NanoMessageBus.Wireup
{
	using Autofac;
	using Autofac.Core;
	using Core;
	using Endpoints;
	using Transports;

	public class TransportWireup : WireupModule
	{
		private int maxThreads = 1;
		private int maxAttempts = 5;

		public TransportWireup(IWireup parent)
			: base(parent)
		{
		}

		public virtual TransportWireup ReceiveWith(int maxWorkerThreads)
		{
			this.maxThreads = maxWorkerThreads;
			return this;
		}
		public virtual TransportWireup AttemptOnFailureAtLeast(int times)
		{
			this.maxAttempts = times;
			return this;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder
				.Register(this.BuildTransport)
				.As<ITransportMessages>()
				.SingleInstance();

			builder
				.Register(this.BuildMessageReceiver)
				.As<IReceiveMessages>()
				.InstancePerDependency()
				.ExternallyOwned();

			builder
				.Register(this.BuildPoisonMessageHandler)
				.As<IForwardPoisonMessages>()
				.SingleInstance()
				.ExternallyOwned();
		}
		protected virtual ITransportMessages BuildTransport(IComponentContext c)
		{
			var threadSafeContext = c.Resolve<IComponentContext>();
			return new MessageQueueTransport(
				() => threadSafeContext.Resolve<IReceiveMessages>(),
				c.Resolve<ISendToEndpoints>(),
				this.maxThreads);
		}
		protected virtual IReceiveMessages BuildMessageReceiver(IComponentContext c)
		{
			var threadSafeContext = c.Resolve<IComponentContext>();
			return new MessageReceiverWorkerThread(
				c.Resolve<IReceiveFromEndpoints>(),
				c.Resolve<IForwardPoisonMessages>(),
				() => threadSafeContext.Resolve<IRouteMessagesToHandlers>(),
				action => new BackgroundThread(() => action()));
		}
		protected virtual IForwardPoisonMessages BuildPoisonMessageHandler(IComponentContext c)
		{
			return new PoisonMessageHandler(
				c.ResolveNamed<ISendToEndpoints>(EndpointWireup.PoisonEndpoint, new Parameter[0]),
				this.maxAttempts);
		}
	}
}