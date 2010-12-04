namespace NanoMessageBus.Transports
{
	using System;
	using Core;
	using Endpoints;
	using Logging;

	public class MessageReceiverWorkerThread : IReceiveMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageReceiverWorkerThread));
		private readonly IReceiveFromEndpoints receiverQueue;
		private readonly IForwardPoisonMessages poisonMessageHandler;
		private readonly Func<IRouteMessagesToHandlers> routerFactory;
		private readonly IThread thread;
		private bool started;

		public MessageReceiverWorkerThread(
			IReceiveFromEndpoints receiverQueue,
			IForwardPoisonMessages poisonMessageHandler,
			Func<IRouteMessagesToHandlers> routerFactory,
			Func<Action, IThread> thread)
		{
			this.receiverQueue = receiverQueue;
			this.poisonMessageHandler = poisonMessageHandler;
			this.routerFactory = routerFactory;
			this.thread = thread(this.BeginReceive);
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
				Log.Info(Diagnostics.MessageProcessed, this.thread.Name);
				this.poisonMessageHandler.Handle(message);
			}
			catch (Exception e)
			{
				Log.Info(Diagnostics.MessageProcessingFailed, this.thread.Name, e.Message);
				this.poisonMessageHandler.HandleFailure(message, e);
			}
		}
	}
}