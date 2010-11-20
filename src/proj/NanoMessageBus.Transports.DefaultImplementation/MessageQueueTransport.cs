namespace NanoMessageBus.Transports
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Core;
	using Endpoints;
	using Logging;

	public class MessageQueueTransport : ITransportMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageQueueTransport));
		private static readonly TimeSpan KillDelay = new TimeSpan(0, 0, 0, 2, 250); // 2.25 seconds
		private readonly ICollection<WorkerThread> workers = new LinkedList<WorkerThread>();
		private readonly IReceiveFromEndpoints receiver;
		private readonly ISendToEndpoints sender;
		private readonly Func<IRouteMessagesToHandlers> router;
		private readonly int maxThreads;
		private bool started;
		private bool stopping;
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
			Log.Info(Diagnostics.TransportDisposed);
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
			if (this.stopping || !this.started)
				return;

			this.stopping = true;
			this.StopWorkerThreads();
			this.KillWorkerThreads();
			this.workers.Clear();
			this.stopping = false;

			this.started = false;
		}

		private void StopWorkerThreads()
		{
			Log.Info(Diagnostics.StoppingWorkers, this.workers.Count);
			foreach (var worker in this.workers)
				worker.Stop();
		}
		private void KillWorkerThreads()
		{
			Thread.Sleep(KillDelay); // TODO: more efficient way if all threads have already exited.
			foreach (var worker in this.workers)
				worker.Kill();
		}
	}
}