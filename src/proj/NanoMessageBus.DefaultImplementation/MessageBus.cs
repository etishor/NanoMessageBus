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
			Log.Debug(Diagnostics.Replying, this.context.Current.ReturnAddress);
			this.Dispatch(
				messages,
				populated => populated.BuildPhysicalMessage(this.context.Current.CorrelationId),
				type => new[] { this.context.Current.ReturnAddress });
		}

		public virtual void Publish(params object[] messages)
		{
			Log.Debug(Diagnostics.Publishing);
			this.Dispatch(
				messages,
				populated => populated.BuildPhysicalMessage(),
				type => this.subscriptions.GetSubscribers(type));
		}

		private void Dispatch(
			IEnumerable<object> messages,
			Func<IEnumerable<object>, PhysicalMessage> getMessage,
			Func<Type, IEnumerable<string>> getRecipients)
		{
			var populated = messages.PopulatedMessagesOnly();
			if (!populated.HasAny())
				return;

			var primaryMessageType = populated.First().GetType();
			var addresses = getRecipients(primaryMessageType).ToArray();

			if (addresses.Length == 0)
			{
				Log.Warn(Diagnostics.DroppingMessage, primaryMessageType);
				return;
			}

			this.transport.Send(getMessage(populated), addresses);
		}
	}
}