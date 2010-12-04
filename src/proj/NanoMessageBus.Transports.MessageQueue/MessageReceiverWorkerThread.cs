namespace NanoMessageBus.Transports
{
	using System;
	using System.Collections.Generic;
	using Core;
	using Endpoints;
	using Logging;

	public class MessageReceiverWorkerThread : IReceiveMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageReceiverWorkerThread));
		private static readonly IDictionary<Guid, int> MessageFailures = new Dictionary<Guid, int>();
		private readonly IReceiveFromEndpoints receiverQueue;
		private readonly ISendToEndpoints poisonQueue;
		private readonly Func<IRouteMessagesToHandlers> routerFactory;
		private readonly IThread thread;
		private readonly int maxAttempts;
		private bool started;

		public MessageReceiverWorkerThread(
			IReceiveFromEndpoints receiverQueue,
			ISendToEndpoints poisonQueue,
			Func<IRouteMessagesToHandlers> routerFactory,
			Func<Action, IThread> thread,
			int maxAttempts)
		{
			this.receiverQueue = receiverQueue;
			this.poisonQueue = poisonQueue;
			this.routerFactory = routerFactory;
			this.thread = thread(this.BeginReceive);
			this.maxAttempts = maxAttempts;
		}

		public virtual void Start()
		{
			if (this.started)
				return;

			this.started = true;

			Log.Info(Diagnostics.StartingWorker, this.thread.Name);
			if (!this.thread.IsAlive)
				this.thread.Start();
		}
		public virtual void Stop()
		{
			if (!this.started)
				return;

			Log.Info(Diagnostics.StoppingWorkerThread, this.thread.Name);
			this.started = false;
		}
		public virtual void Abort()
		{
			if (!this.thread.IsAlive)
				return;

			Log.Info(Diagnostics.AbortingWorkerThread, this.thread.Name);
			this.thread.Abort();
		}

		protected virtual void BeginReceive()
		{
			while (this.started)
				this.Receive();
		}
		protected virtual void Receive()
		{
			using (var router = this.routerFactory())
				this.RouteToHandlers(router, this.receiverQueue.Receive());
		}
		private void RouteToHandlers(IRouteMessagesToHandlers router, TransportMessage message)
		{
			if (!message.IsPopulated())
				return;

			Log.Info(Diagnostics.DispatchingToRouter, this.thread.Name, router.GetType());

			try
			{
				router.Route(message);
				MessageFailures.Remove(message.MessageId);
				Log.Info(Diagnostics.MessageProcessed, this.thread.Name);
			}
			catch (Exception e)
			{
				this.ForwardToPoisonQueue(message, e);
			}
		}
		private void ForwardToPoisonQueue(TransportMessage message, Exception error)
		{
			Log.Info(Diagnostics.MessageProcessingFailed, this.thread.Name, error.Message);
			if (MessageFailures.Increment(message.MessageId) < this.maxAttempts)
				return;

			Log.Info(Diagnostics.ForwardingMessageToPoisonMessageQueue, this.maxAttempts, message.MessageId);
			AppendExceptionHeaders(message, error, 0);
			this.poisonQueue.Send(message);
		}
		private static void AppendExceptionHeaders(TransportMessage message, Exception error, int depth)
		{
			message.Headers["ExceptionMessage" + depth] = error.Message;
			message.Headers["ExceptionStackTrace" + depth] = error.StackTrace;
			message.Headers["ExceptionSource" + depth] = error.Source;

			if (error.InnerException != null)
				AppendExceptionHeaders(message, error.InnerException, ++depth);
		}
	}
}