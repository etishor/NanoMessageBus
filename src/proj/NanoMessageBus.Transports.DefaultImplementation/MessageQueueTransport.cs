namespace NanoMessageBus.Transports
{
	using System;
	using System.Collections.Generic;
	using Core;
	using Endpoints;
	using Logging;

	public class MessageQueueTransport : ITransportMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageQueueTransport));
		private readonly ICollection<WorkerThread> workers = new LinkedList<WorkerThread>();
		private readonly IReceiveFromEndpoints receiver;
		private readonly ISendToEndpoints sender;
		private readonly Func<IRouteMessagesToHandlers> router;
		private readonly int maxThreads;
		private bool started;
		private bool disposed;

		public MessageQueueTransport(
			IReceiveFromEndpoints receiver,
			ISendToEndpoints sender,
			Func<IRouteMessagesToHandlers> router,
			int maxThreads)
		{
			this.receiver = receiver;
			this.router = router;
			this.sender = sender;
			this.maxThreads = maxThreads;
		}
		~MessageQueueTransport()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.Dispose(true);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed || !disposing)
				return;

			this.disposed = true;

			Log.Info(Diagnostics.DisposingTransport);
			this.StopListening();
		}

		public virtual void Send(PhysicalMessage message, params string[] recipients)
		{
			if (!message.IsPopulated())
				return;

			Log.Debug(Diagnostics.SendingMessage);
			this.sender.Send(message, recipients);
		}

		public virtual void StartListening()
		{
			if (this.started)
				return;

			this.started = true;

			Log.Info(Diagnostics.InitializingWorkers, this.maxThreads);
			while (this.workers.Count < this.maxThreads)
				this.AddWorkerThread().Start();
		}
		protected virtual WorkerThread AddWorkerThread()
		{
			var worker = new WorkerThread(this.receiver, this.router);
			this.workers.Add(worker);
			return worker;
		}

		public virtual void StopListening()
		{
			if (!this.started)
				return;

			this.started = false;

			Log.Info(Diagnostics.StoppingWorkers, this.workers.Count);

			foreach (var worker in this.workers)
				worker.Dispose();

			this.workers.Clear();
		}
	}
}