namespace NanoMessageBus.Wireup
{
	using Autofac;
	using Endpoints;
	using Serialization;

	public class EndpointWireup : WireupModule
	{
		private string receiverAddress;
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
		}
		protected virtual ISendToEndpoints BuildSenderEndpoint(IComponentContext c)
		{
			return new MsmqSenderEndpoint(
				address => MsmqConnector.OpenSend(new MsmqAddress(address), this.enlist),
				c.Resolve<ISerializeMessages>());
		}
		protected virtual IReceiveFromEndpoints BuildReceiverEndpoint(IComponentContext c)
		{
			var address = new MsmqAddress(this.receiverAddress);
			return new MsmqReceiverEndpoint(
				MsmqConnector.OpenReceive(address, this.enlist),
				c.Resolve<ISerializeMessages>());
		}
	}
}