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
		private readonly IReceiveFromEndpoints receiverEndpoint;
		private readonly ISendToEndpoints senderEndpoint;
		private readonly Func<IReceiveMessages> messageReceiver;
		private readonly int maxThreads;
		private bool started;
		private bool disposed;

		public MessageQueueTransport(
			IReceiveFromEndpoints receiverEndpoint,
			ISendToEndpoints senderEndpoint,
			Func<IReceiveMessages> messageReceiver,
			int maxThreads)
		{
			this.receiverEndpoint = receiverEndpoint;
			this.messageReceiver = messageReceiver;
			this.senderEndpoint = senderEndpoint;
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
			this.Stop();
		}

		public virtual void Send(PhysicalMessage message, params string[] recipients)
		{
			if (message.IsPopulated())
				this.senderEndpoint.Send(message, recipients);

			Log.Debug(Diagnostics.SendingMessage);

			this.senderEndpoint.Send(message, recipients);
		}

		public virtual void Start()
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
			var worker = new WorkerThread(this.receiverEndpoint, this.messageReceiver);
			this.workers.Add(worker);
			return worker;
		}

		public virtual void Stop()
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