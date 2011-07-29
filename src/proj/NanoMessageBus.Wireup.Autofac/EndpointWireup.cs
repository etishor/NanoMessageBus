namespace NanoMessageBus.Wireup
{
    using System;
    using Autofac;
    using Autofac.Core;
    using Core;
    using Endpoints;
    using Serialization;

    public class EndpointWireup : WireupModule
    {
        private const string PoisonEndpoint = "PoisonEndpoint";
        private const bool PoisonEndpointEnlist = false;
        private string receiverAddress;
        private string poisonAddress;
        private int maxRetries = 3;
        private bool enlist = true;
        private Action<EnvelopeMessage> postHandlingAction;

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

        public virtual EndpointWireup PostPoisonMessageHandlingAction(Action<EnvelopeMessage> postHandlingAction)
        {
            this.postHandlingAction = postHandlingAction;
            return this;
        }

        public virtual EndpointWireup RetryAtLeast(int times)
        {
            this.maxRetries = times;
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

            builder
                .Register(this.BuildPoisonMessageHandler)
                .As<IHandlePoisonMessages>()
                .SingleInstance()
                .ExternallyOwned();
        }
        protected virtual ISendToEndpoints BuildSenderEndpoint(IComponentContext c)
        {
            return new MsmqSenderEndpoint(
                address => MsmqConnector.OpenSend(new MsmqAddress(address.ToString()), this.enlist),
                c.Resolve<ISerializeMessages>());
        }
        protected virtual IReceiveFromEndpoints BuildReceiverEndpoint(IComponentContext c)
        {
            return new MsmqReceiverEndpoint(
                MsmqConnector.OpenReceive(new MsmqAddress(this.receiverAddress), this.enlist),
                MsmqConnector.OpenSend(new MsmqAddress(this.poisonAddress), this.enlist),
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
        protected virtual IHandlePoisonMessages BuildPoisonMessageHandler(IComponentContext c)
        {
            if (!this.enlist)
                return new NonTransactionalPoisonMessageHandler();

            return new PoisonMessageHandler(
                c.ResolveNamed<ISendToEndpoints>(PoisonEndpoint, new Parameter[0]), new Uri(this.poisonAddress), this.maxRetries,this.postHandlingAction);
        }
    }
}