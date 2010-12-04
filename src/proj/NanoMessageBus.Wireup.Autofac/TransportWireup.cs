namespace NanoMessageBus.Wireup
{
	using Autofac;
	using Core;
	using Endpoints;
	using Transports;

	public class TransportWireup : WireupModule
	{
		private int maxThreads = 1;

		public TransportWireup(IWireup parent)
			: base(parent)
		{
		}

		public virtual TransportWireup ReceiveWith(int maxWorkerThreads)
		{
			this.maxThreads = maxWorkerThreads;
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
				() => threadSafeContext.Resolve<IRouteMessagesToHandlers>(),
				action => new BackgroundThread(() => action()));
		}
	}
}