namespace NanoMessageBus.MessageSubscriber
{
	using System;
	using System.Collections.Generic;
	using Logging;
	using Transports;

	public class MessageSubscriber : ISubscribeToMessages, IUnsubscribeFromMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageSubscriber));
		private readonly string returnAddress;
		private readonly ITransportMessages transport;

		public MessageSubscriber(string returnAddress, ITransportMessages transport)
		{
			this.returnAddress = returnAddress;
			this.transport = transport;
		}

		public virtual void Subscribe(string endpointAddress, DateTime expiration, params Type[] messageTypes)
		{
			var request = BuildRequest(expiration, messageTypes);

			foreach (var messageType in request.MessageTypes)
				Log.Info(Diagnostics.Subscribe, this.returnAddress, endpointAddress, expiration, messageType);

			this.Send(request, endpointAddress);
		}
		private static SubscriptionRequestMessage BuildRequest(DateTime expiration, IEnumerable<Type> types)
		{
			return new SubscriptionRequestMessage
			{
				MessageTypes = types.GetTypeNames(),
				Expiration = expiration
			};
		}

		public virtual void Unsubscribe(string endpointAddress, params Type[] messageTypes)
		{
			var request = BuildRequest(messageTypes);

			foreach (var messageType in request.MessageTypes)
				Log.Info(Diagnostics.Unsubscribe, this.returnAddress, endpointAddress, messageType);

			this.Send(request, endpointAddress);
		}
		private static UnsubscribeRequestMessage BuildRequest(IEnumerable<Type> types)
		{
			return new UnsubscribeRequestMessage
			{
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
				this.returnAddress,
				TimeSpan.MaxValue,
				true,
				null,
				new[] { logicalMessage });
		}
	}
}