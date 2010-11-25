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
		private readonly MessageBuilder builder;

		public MessageBus(
			ITransportMessages transport,
			IStoreSubscriptions subscriptions,
			IDictionary<Type, IEnumerable<string>> recipients,
			IMessageContext context,
			MessageBuilder builder)
		{
			this.transport = transport;
			this.subscriptions = subscriptions;
			this.recipients = recipients;
			this.context = context;
			this.builder = builder;
		}

		public virtual void Send(params object[] messages)
		{
			Log.Debug(Diagnostics.SendingMessage);
			this.Dispatch(messages, msg => this.recipients.GetRecipients(msg.GetTypes()));
		}
		public virtual void Reply(params object[] messages)
		{
			Log.Debug(Diagnostics.Replying, this.context.CurrentMessage.ReturnAddress);
			this.Dispatch(messages, msg => new[] { this.context.CurrentMessage.ReturnAddress });
		}
		public virtual void Publish(params object[] messages)
		{
			Log.Debug(Diagnostics.Publishing);
			this.Dispatch(messages, msg => this.subscriptions.GetSubscribers(msg.GetTypes().GetTypeNames()));
		}

		private void Dispatch(object[] messages, Func<object, IEnumerable<string>> getRecipients)
		{
			messages = messages.PopulatedMessagesOnly();
			if (!messages.Any())
				return;

			var addresses = getRecipients(messages[0]).Where(x => !string.IsNullOrEmpty(x)).ToArray();
			if (addresses.Length == 0)
			{
				Log.Warn(Diagnostics.DroppingMessage, messages[0].GetType());
				return;
			}

			this.transport.Send(this.builder.BuildMessage(messages), addresses);
		}
	}
}