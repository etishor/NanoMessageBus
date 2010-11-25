namespace NanoMessageBus.MessageSubscriber
{
	using System;
	using System.Collections.Generic;
	using Transports;

	public class MessageSubscriber : ISubscribeToMessages, IUnsubscribeFromMessages
	{
		private readonly string localAddress;
		private readonly ITransportMessages transport;

		public MessageSubscriber(string localAddress, ITransportMessages transport)
		{
			this.localAddress = localAddress;
			this.transport = transport;
		}

		public virtual void Subscribe(string endpointAddress, DateTime expiration, params Type[] messageTypes)
		{
			var request = this.BuildSubscriptionRequest(expiration, messageTypes);
			this.Send(request, endpointAddress);
		}
		private SubscriptionRequestMessage BuildSubscriptionRequest(DateTime expiration, IEnumerable<Type> types)
		{
			return new SubscriptionRequestMessage
			{
				Subscriber = this.localAddress,
				MessageTypes = types.GetTypeNames(),
				Expiration = expiration
			};
		}

		public virtual void Unsubscribe(string endpointAddress, params Type[] messageTypes)
		{
			var request = this.BuildUnsubscribeRequest(messageTypes);
			this.Send(request, endpointAddress);
		}
		private UnsubscribeRequestMessage BuildUnsubscribeRequest(IEnumerable<Type> types)
		{
			return new UnsubscribeRequestMessage
			{
				Subscriber = this.localAddress,
				MessageTypes = types.GetTypeNames()
			};
		}

		private void Send(object request, string endpointAddress)
		{
			var physicalMessage = this.BuildPhysicalMessage(request);
			this.transport.Send(physicalMessage, endpointAddress);
		}
		private PhysicalMessage BuildPhysicalMessage(object logicalMessage)
		{
			return new PhysicalMessage(
				Guid.NewGuid(),
				this.localAddress,
				TimeSpan.MaxValue,
				true,
				null,
				new[] { logicalMessage });
		}
	}
}