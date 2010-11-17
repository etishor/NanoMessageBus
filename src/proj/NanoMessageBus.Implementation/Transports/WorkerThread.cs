namespace NanoMessageBus.Transports
{
	using System;
	using System.Threading;
	using Core;
	using Endpoints;
	using Logging;

	public class WorkerThread
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(WorkerThread));
		private readonly IReceiveFromEndpoints receiver;
		private readonly Func<IReceiveMessages> messageReceiver;
		private readonly Thread thread;
		private bool started;
		private bool disposed;

		public WorkerThread(IReceiveFromEndpoints receiver, Func<IReceiveMessages> messageReceiver)
		{
			this.receiver = receiver;
			this.messageReceiver = messageReceiver;
			this.thread = new Thread(this.BeginReceive)
			{
				IsBackground = true
			};

			this.thread.Name = TransportDiagnostics.WorkerThreadName.FormatWith(this.thread.ManagedThreadId);
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

			Log.Info(TransportDiagnostics.StoppingWorker, this.thread.Name);

			this.started = false;
			this.disposed = true;
		}

		public virtual void Start()
		{
			if (this.started)
				return;

			lock (this.thread)
			{
				if (this.started)
					return;

				this.started = true;

				Log.Info(TransportDiagnostics.StartingWorker, this.thread.Name);
				if (!this.thread.IsAlive)
					this.thread.Start();
			}
		}
		protected virtual void BeginReceive()
		{
			while (this.started)
				this.Receive();
		}
		protected virtual void Receive()
		{
			var message = this.receiver.Receive();
			if (!message.IsPopulated())
				return;

			using (var handler = this.messageReceiver())
			{
				Log.Info(TransportDiagnostics.DispatchingToReceiver, this.thread.Name, handler.GetType());
				handler.Receive(message);
			}

			Log.Info(TransportDiagnostics.MessageProcessed, this.thread.Name);
		}
	}
}