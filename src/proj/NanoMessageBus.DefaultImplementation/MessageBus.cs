namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Logging;
	using SubscriptionStorage;
	using Transports;

	public class MessageBus : ISendMessages, IPublishMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageBus));
		private readonly ITransportMessages transport;
		private readonly IStoreSubscriptions subscriptions;
		private readonly IDictionary<Type, IEnumerable<string>> recipients;
		private readonly IMessageContext context;

		public MessageBus(
			ITransportMessages transport,
			IStoreSubscriptions subscriptions,
			IDictionary<Type, IEnumerable<string>> recipients,
			IMessageContext context)
		{
			this.transport = transport;
			this.context = context;
			this.recipients = recipients;
			this.subscriptions = subscriptions;
		}

		public virtual void Send(params object[] messages)
		{
			Log.Debug(Diagnostics.SendingMessage);
			this.Dispatch(
				messages,
				populated => populated.BuildPhysicalMessage(),
				type => this.recipients.GetRecipients(type));
		}

		public virtual void Reply(params object[] messages)
		{
			Log.Debug(Diagnostics.Replying, this.context.CurrentMessage.ReturnAddress);
			this.Dispatch(
				messages,
				populated => populated.BuildPhysicalMessage(this.context.CurrentMessage.CorrelationId),
				type => new[] { this.context.CurrentMessage.ReturnAddress });
		}

		public virtual void Publish(params object[] messages)
		{
			Log.Debug(Diagnostics.Publishing);
			this.Dispatch(
				messages,
				populated => populated.BuildPhysicalMessage(),
				type => this.subscriptions.GetSubscribers(new[] { type }));
		}

		private void Dispatch(
			IEnumerable<object> messages,
			Func<IEnumerable<object>, PhysicalMessage> buildMessage,
			Func<Type, IEnumerable<string>> getMessageRecipients)
		{
			var logicalMessages = messages.PopulatedMessagesOnly();
			if (!logicalMessages.HasAny())
				return;

			var primaryLogicalMessageType = logicalMessages.First().GetType();
			var addresses = getMessageRecipients(primaryLogicalMessageType).ToArray();

			if (addresses.Length == 0)
			{
				Log.Warn(Diagnostics.DroppingMessage, primaryLogicalMessageType);
				return;
			}

			this.transport.Send(buildMessage(logicalMessages), addresses);
		}
	}
}