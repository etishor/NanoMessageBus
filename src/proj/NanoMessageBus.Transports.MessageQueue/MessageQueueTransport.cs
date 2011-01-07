namespace NanoMessageBus.Transports
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Endpoints;
	using Logging;

	public class MessageQueueTransport : ITransportMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageQueueTransport));
		private static readonly TimeSpan KillDelay = 750.Milliseconds();
		private readonly ICollection<IReceiveMessages> workers = new LinkedList<IReceiveMessages>();
		private readonly Func<IReceiveMessages> receiverFactory;
		private readonly ISendToEndpoints sender;
		private readonly int maxThreads;
		private bool started;
		private bool stopping;
		private bool disposed;

		public MessageQueueTransport(
			Func<IReceiveMessages> receiverFactory, ISendToEndpoints sender, int maxThreads)
		{
			this.receiverFactory = receiverFactory;
			this.sender = sender;
			this.maxThreads = maxThreads;
		}
		~MessageQueueTransport()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
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

		public virtual void Send(EnvelopeMessage message, params Uri[] recipients)
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
		protected virtual IReceiveMessages AddWorkerThread()
		{
			var worker = this.receiverFactory();
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
			// ENHANCEMENT: There's a more efficient way to do this if all of the threads have exited.
			Thread.Sleep(KillDelay);
			foreach (var worker in this.workers)
				worker.Abort();
		}
	}
}