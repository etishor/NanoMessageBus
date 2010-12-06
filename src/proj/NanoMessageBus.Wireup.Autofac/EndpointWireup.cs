namespace NanoMessageBus.Wireup
{
	using Autofac;
	using Endpoints;
	using Serialization;

	public class EndpointWireup : WireupModule
	{
		internal const string PoisonEndpoint = "PoisonEndpoint";
		private const bool PoisonEndpointEnlist = false;
		private string receiverAddress;
		private string poisonAddress;
		private bool enlist = true;

		public EndpointWireup(IWireup parent)
			: base(parent)
		{
		}

		public virtual EndpointWireup ListenOn(string address)
		{
			this.receiverAddress = address;
			return this;
		}
		public virtual EndpointWireup ForwardPoisonMessagesTo(string address)
		{
			this.poisonAddress = address;
			return this;
		}
		public virtual EndpointWireup IgnoreTransactions()
		{
			this.enlist = false;
			return this;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder
				.Register(this.BuildReceiverEndpoint)
				.As<IReceiveFromEndpoints>()
				.SingleInstance();

			builder
				.Register(this.BuildSenderEndpoint)
				.As<ISendToEndpoints>()
				.SingleInstance();

			builder
				.Register(this.BuildPoisonEndpoint)
				.Named(PoisonEndpoint, typeof(ISendToEndpoints))
				.SingleInstance();
		}
		protected virtual ISendToEndpoints BuildSenderEndpoint(IComponentContext c)
		{
			return new MsmqSenderEndpoint(
				address => MsmqConnector.OpenSend(new MsmqAddress(address), this.enlist),
				c.Resolve<ISerializeMessages>());
		}
		protected virtual ISendToEndpoints BuildPoisonEndpoint(IComponentContext c)
		{
			// ignore address provided, always send to the configured poison address
			var address = new MsmqAddress(this.poisonAddress);
			return new MsmqSenderEndpoint(
				addr => MsmqConnector.OpenSend(address, PoisonEndpointEnlist),
				c.Resolve<ISerializeMessages>());
		}
		protected virtual IReceiveFromEndpoints BuildReceiverEndpoint(IComponentContext c)
		{
			return new MsmqReceiverEndpoint(
				MsmqConnector.OpenReceive(new MsmqAddress(this.receiverAddress), this.enlist),
				MsmqConnector.OpenSend(new MsmqAddress(this.poisonAddress), this.enlist),
				c.Resolve<ISerializeMessages>());
		}
	}
}