namespace NanoMessageBus.MessageQueueTransport
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Core;
	using Endpoints;
	using Transport;

	public class DefaultTransport : ITransportMessages
	{
		private readonly ICollection<WorkerThread> workers = new LinkedList<WorkerThread>();
		private readonly IReceiveFromEndpoints receiverEndpoint;
		private readonly ISendToEndpoints senderEndpoint;
		private readonly Func<IReceiveMessages> messageReceiver;
		private readonly int maxThreads;
		private bool started;
		private bool disposed;

		public DefaultTransport(
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
		~DefaultTransport()
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

			this.Stop();
			this.disposed = true;
		}

		public virtual void Send(PhysicalMessage message, params string[] recipients)
		{
			if (message.IsPopulated())
				this.senderEndpoint.Send(message, recipients);

			this.senderEndpoint.Send(message, recipients);
		}

		public virtual void Start()
		{
			if (this.started)
				return;

			lock (this.workers)
			{
				if (this.started)
					return;

				this.started = true;

				while (this.workers.Count < this.maxThreads)
					this.AddWorkerThread().Start();
			}
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

			lock (this.workers)
			{
				if (!this.started)
					return;

				this.started = false;

				foreach (var worker in this.workers)
					worker.Dispose();

				this.workers.Clear();
			}
		}
	}
}