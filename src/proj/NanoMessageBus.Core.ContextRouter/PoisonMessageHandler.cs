namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;
	using Endpoints;
	using Logging;

	public class PoisonMessageHandler : IHandlePoisonMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(PoisonMessageHandler));
		private readonly IDictionary<Guid, int> messageFailures = new Dictionary<Guid, int>();
		private readonly ISendToEndpoints poisonQueue;
		private readonly int maxAttempts;

		public PoisonMessageHandler(ISendToEndpoints poisonQueue, int maxAttempts)
		{
			this.poisonQueue = poisonQueue;
			this.maxAttempts = maxAttempts;
		}

		public virtual void Handle(TransportMessage message)
		{
			if (message != null)
				this.messageFailures.Remove(message.MessageId);
		}
		public virtual void HandleFailure(TransportMessage message, Exception exception)
		{
			if (!this.ReachedMaxAttempts(message))
				return;

			Log.Info(Diagnostics.ForwardingMessageToPoisonMessageQueue, this.maxAttempts, message.MessageId);
			AppendExceptionHeaders(message, exception, 0);
			this.poisonQueue.Send(message);
		}
		private bool ReachedMaxAttempts(TransportMessage message)
		{
			lock (this.messageFailures)
			{
				int attempts;
				this.messageFailures.TryGetValue(message.MessageId, out attempts);
				this.messageFailures[message.MessageId] = ++attempts;
				return attempts >= this.maxAttempts;
			}
		}
		private static void AppendExceptionHeaders(TransportMessage message, Exception exception, int depth)
		{
			if (null == exception)
				return;

			message.Headers["ExceptionMessage" + depth] = exception.Message;
			message.Headers["ExceptionStackTrace" + depth] = exception.StackTrace;
			message.Headers["ExceptionSource" + depth] = exception.Source;

			AppendExceptionHeaders(message, exception.InnerException, ++depth);
		}
	}
}