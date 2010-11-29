namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;
	using Logging;
	using SubscriptionStorage;
	using Transports;

	public class MessageBus : ISendMessages, IPublishMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageBus));
		private readonly ITransportMessages transport;
		private readonly IStoreSubscriptions subscriptions;
		private readonly IDictionary<Type, ICollection<string>> recipients;
		private readonly IMessageContext context;
		private readonly MessageBuilder builder;
		private readonly IDiscoverMessageTypes discoverer;

		public MessageBus(
			ITransportMessages transport,
			IStoreSubscriptions subscriptions,
			IDictionary<Type, ICollection<string>> recipients,
			IMessageContext context,
			MessageBuilder builder,
			IDiscoverMessageTypes discoverer)
		{
			this.transport = transport;
			this.subscriptions = subscriptions;
			this.recipients = recipients;
			this.context = context;
			this.builder = builder;
			this.discoverer = discoverer;
		}

		public virtual void Send(params object[] messages)
		{
			Log.Debug(Diagnostics.SendingMessage);
			this.Dispatch(messages, msg => this.GetRecipients(this.discoverer.GetTypes(msg)));
		}
		public ICollection<string> GetRecipients(IEnumerable<Type> messageTypes)
		{
			ICollection<string> list = new HashSet<string>();

			foreach (var messageType in messageTypes)
			{
				ICollection<string> recipientsForMessageType;
				if (!this.recipients.TryGetValue(messageType, out recipientsForMessageType))
					continue;

				foreach (var recipient in recipientsForMessageType)
					list.Add(recipient);
			}

			return list;
		}

		public virtual void Reply(params object[] messages)
		{
			Log.Debug(Diagnostics.Replying, this.context.CurrentMessage.ReturnAddress);
			this.Dispatch(messages, msg => new[] { this.context.CurrentMessage.ReturnAddress });
		}

		public virtual void Publish(params object[] messages)
		{
			Log.Debug(Diagnostics.Publishing);
			this.Dispatch(messages, GetSubscribers);
		}
		private IEnumerable<string> GetSubscribers(object message)
		{
			return this.subscriptions.GetSubscribers(this.discoverer.GetTypeNames(message));
		}

		private void Dispatch(object[] messages, Func<object, IEnumerable<string>> getRecipients)
		{
			messages = PopulatedMessagesOnly(messages);
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
		private static object[] PopulatedMessagesOnly(object[] messages)
		{
			messages = (messages ?? new object[] { }).Where(x => x != null).ToArray();
			return messages;
		}
	}
}