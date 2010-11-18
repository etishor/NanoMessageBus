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

			Log.Debug(Diagnostics.SendingMessages, messages.Length);

			this.transport.Send(
				messages.BuildPhysicalMessage(this.context),
				this.GetRecipients(messages));
		}
		private string[] GetRecipients(IEnumerable<object> messages)
		{
			var firstMessage = messages.First(x => x != null);
			IEnumerable<string> recipientsForMessageType;
			if (!this.recipients.TryGetValue(firstMessage.GetType(), out recipientsForMessageType))
				Log.Warn(Diagnostics.NoRecipientsFound, firstMessage.GetType());

			return new List<string>(recipientsForMessageType).ToArray();
		}

		public virtual void Reply(params object[] messages)
		{
			if (!messages.HasMessages())
				return;

			Log.Debug(Diagnostics.ReplyingToReturnAddress, messages.Length, this.context.Current.ReturnAddress);

			this.transport.Send(
				messages.BuildPhysicalMessage(this.context),
				this.context.Current.ReturnAddress);
		}
	}
}