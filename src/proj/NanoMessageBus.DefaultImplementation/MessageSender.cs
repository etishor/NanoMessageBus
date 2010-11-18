namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;
	using Logging;
	using Transports;

	public class MessageSender : ISendMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageSender));
		private readonly IMessageContext context;
		private readonly ITransportMessages transport;
		private readonly IDictionary<Type, IEnumerable<string>> recipients;

		public MessageSender(
			IMessageContext context,
			ITransportMessages transport,
			IDictionary<Type, IEnumerable<string>> recipients)
		{
			this.context = context;
			this.recipients = recipients;
			this.transport = transport;
		}

		public virtual void Send(params object[] messages)
		{
			if (!messages.HasMessages())
				return;

			this.transport.Send(
				messages.BuildPhysicalMessage(this.context),
				this.GetRecipients(messages));
		}
		private string[] GetRecipients(IEnumerable<object> messages)
		{
			ICollection<string> addresses = new LinkedList<string>();

			foreach (var message in messages)
			{
				IEnumerable<string> recipientsForMessageType = null;
				if (!this.recipients.TryGetValue(message.GetType(), out recipientsForMessageType))
				{
					Log.Warn("No receipients found for messages of type '{0}'.", message.GetType());
					continue;
				}

				foreach (var recipient in recipientsForMessageType)
					addresses.Add(recipient);
			}

			Log.Verbose("Found '{0}' recipients for the messages provided.", addresses.Count);
			return addresses.ToArray();
		}

		public virtual void Reply(params object[] messages)
		{
			if (!messages.HasMessages())
				return;

			this.transport.Send(
				messages.BuildPhysicalMessage(this.context),
				this.context.Current.ReturnAddress);
		}
	}
}