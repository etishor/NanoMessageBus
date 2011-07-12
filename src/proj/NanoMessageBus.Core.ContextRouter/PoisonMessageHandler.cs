namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;
	using Endpoints;
	using Logging;

	public class PoisonMessageHandler : IHandlePoisonMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(PoisonMessageHandler));
        private readonly Uri PoisonMessageQueue = null; // null == set by configuration
		private readonly IDictionary<Guid, int> messageFailures = new Dictionary<Guid, int>();
		private readonly ISendToEndpoints poisonQueue;
		private readonly int maxRetries;

		public PoisonMessageHandler(ISendToEndpoints poisonQueue, Uri poisonMessageQueue, int maxRetries)
		{
			this.poisonQueue = poisonQueue;
			this.maxRetries = maxRetries;
            this.PoisonMessageQueue = poisonMessageQueue;
		}

		public virtual bool IsPoison(EnvelopeMessage message)
		{
			if (message == null)
				return false;

			int retries;
			this.messageFailures.TryGetValue(message.MessageId, out retries);
			return retries >= this.maxRetries;
		}

		public virtual void ClearFailures(EnvelopeMessage message)
		{
			if (message != null)
				this.messageFailures.Remove(message.MessageId);
		}

		public virtual void HandleFailure(EnvelopeMessage message, Exception exception)
		{
			if (this.CanRetry(message))
				return;

			AppendExceptionHeaders(message, exception, 0);
			this.ForwardToPoisonMessageQueue(message);
		}
		private bool CanRetry(EnvelopeMessage message)
		{
			lock (this.messageFailures)
			{
				int retries;
				this.messageFailures.TryGetValue(message.MessageId, out retries);
				this.messageFailures[message.MessageId] = ++retries;
				return retries < this.maxRetries;
			}
		}
		private static void AppendExceptionHeaders(EnvelopeMessage message, Exception exception, int depth)
		{
			if (null == exception)
				return;

			message.Headers["ExceptionMessage" + depth] = exception.Message;
			message.Headers["ExceptionStackTrace" + depth] = exception.StackTrace;
			message.Headers["ExceptionSource" + depth] = exception.Source;

			AppendExceptionHeaders(message, exception.InnerException, ++depth);
		}
		private void ForwardToPoisonMessageQueue(EnvelopeMessage message)
		{
			Log.Info(Diagnostics.ForwardingMessageToPoisonMessageQueue, this.maxRetries, message.MessageId);
            this.poisonQueue.Send(message, PoisonMessageQueue);	
		}
	}
}