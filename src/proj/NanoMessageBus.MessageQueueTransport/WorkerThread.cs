namespace NanoMessageBus.MessageQueueTransport
{
	using System;
	using System.Threading;
	using Core;
	using Endpoints;

	public class WorkerThread
	{
		private readonly IReceiveFromEndpoints receiver;
		private readonly Func<IReceiveMessages> messageReceiver;
		private readonly Thread thread;
		private bool started;
		private bool disposed;

		public WorkerThread(IReceiveFromEndpoints receiver, Func<IReceiveMessages> messageReceiver)
		{
			this.receiver = receiver;
			this.messageReceiver = messageReceiver;
			this.thread = new Thread(this.Process)
			{
				Name = string.Format("Worker {0}", this.thread.ManagedThreadId),
				IsBackground = true
			};
		}
		~WorkerThread()
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

			this.started = false;
			this.disposed = true;
		}

		public virtual void Start()
		{
			lock (this.thread)
			{
				if (!this.thread.IsAlive)
					this.thread.Start();
			}
		}
		protected virtual void Process()
		{
			this.started = true;
			while (this.started)
				this.Receive();
		}
		protected virtual void Receive()
		{
			var message = this.receiver.Receive();
			if (!message.IsPopulated())
				return;

			using (var handler = this.messageReceiver())
				handler.Receive(message);
		}
	}
}