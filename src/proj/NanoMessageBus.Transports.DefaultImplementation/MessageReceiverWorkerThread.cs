namespace NanoMessageBus.Transports
{
	using System;
	using System.Threading;
	using Core;
	using Endpoints;
	using Logging;

	public class MessageReceiverWorkerThread : IReceiveMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageReceiverWorkerThread));
		private readonly IReceiveFromEndpoints receiver;
		private readonly Func<IRouteMessagesToHandlers> routerFactory;
		private readonly Thread thread;
		private bool started;

		public MessageReceiverWorkerThread(IReceiveFromEndpoints receiver, Func<IRouteMessagesToHandlers> routerFactory)
		{
			this.receiver = receiver;
			this.routerFactory = routerFactory;
			this.thread = new Thread(this.BeginReceive)
			{
				IsBackground = true
			};

			this.thread.Name = Diagnostics.WorkerThreadName.FormatWith(this.thread.ManagedThreadId);
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
			{
				var message = this.receiver.Receive();
				if (!message.IsPopulated())
					return;

				Log.Info(Diagnostics.DispatchingToReceiver, this.thread.Name, router.GetType());
				router.Route(message);
				Log.Info(Diagnostics.MessageProcessed, this.thread.Name);
			}
		}
	}
}