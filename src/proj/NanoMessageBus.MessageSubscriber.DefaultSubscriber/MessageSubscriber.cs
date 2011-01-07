namespace NanoMessageBus.MessageSubscriber
{
	using System;
	using System.Collections.Generic;
	using Logging;
	using Transports;

	public class MessageSubscriber : ISubscribeToMessages, IUnsubscribeFromMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageSubscriber));
		private readonly Uri returnAddress;
		private readonly ITransportMessages transport;

		public MessageSubscriber(Uri returnAddress, ITransportMessages transport)
		{
			this.returnAddress = returnAddress;
			this.transport = transport;
		}

		public virtual void Subscribe(Uri endpointAddress, DateTime expiration, params Type[] messageTypes)
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

		public virtual void Unsubscribe(Uri endpointAddress, params Type[] messageTypes)
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

		private void Send(object request, Uri endpointAddress)
		{
			var envelopeMessage = this.BuildEnvelopeMessage(request);
			this.transport.Send(envelopeMessage, endpointAddress);
		}
		private EnvelopeMessage BuildEnvelopeMessage(object logicalMessage)
		{
			return new EnvelopeMessage(
				Guid.NewGuid(),
				this.returnAddress,
				TimeSpan.MaxValue,
				true,
				null,
				new[] { logicalMessage });
		}
	}
}