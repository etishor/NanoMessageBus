namespace SendReceive
{
	using System;
	using Autofac;
	using NanoMessageBus.MessageSubscriber;
	using NanoMessageBus.Wireup;

	public class ConfigModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			IWireup wireup = new WireupModule();

			wireup = wireup.Configure<LoggingWireup>()
						.UseOutputWindow();

			wireup = wireup.Configure<TransportWireup>()
						.ReceiveWith(1.Threads());

			wireup = wireup.Configure<SerializationWireup>()
						.CompressMessages()
						.EncryptMessages(new byte[16])
						.ProtocolBufferSerializer(typeof(SubscriptionRequestMessage), typeof(MyMessage));

			wireup = wireup.Configure<EndpointWireup>()
						.ListenOn("msmq://./MyReceiverQueue")
						.IgnoreTransactions();

			wireup = wireup.Configure<SubscriptionStorageWireup>()
						.ConnectTo("SubscriptionStorage");

			wireup = wireup.Configure<MessageSubscriberWireup>()
						.AddSubscription("msmq://./MyReceiverQueue", typeof(MyMessage));

			wireup = wireup.Configure<MessageRouterWireup>();

			wireup = wireup.Configure<MessageBusWireup>()
						.RegisterMessageEndpoint("msmq://./MyReceiverQueue", typeof(MyMessage))
						.RegisterMessageTimeToLive(TimeSpan.MaxValue, typeof(MyMessage))
						.RegisterTransientMessage(typeof(MyMessage));

			wireup = wireup.Configure<MessageHandlerWireup>()
						.AddHandler(new MyMessageHandler());

			wireup.Register(builder);
		}
	}
}