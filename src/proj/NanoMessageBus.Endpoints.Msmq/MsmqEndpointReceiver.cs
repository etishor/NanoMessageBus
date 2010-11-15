namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;
	using Logging;
	using Transport;

	public class MsmqEndpointReceiver : IReceiveFromEndpoints
	{
		public event EventHandler MessageAvailable;

		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqEndpointReceiver));
		private static readonly TimeSpan ReceiveMessageTimeout = 2.Seconds();
		private readonly MsmqProxy inputQueue;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqEndpointReceiver(MsmqProxy inputQueue, ISerializeMessages serializer)
		{
			this.serializer = serializer;
			this.inputQueue = inputQueue;

			this.inputQueue.PeekCompleted += this.OnPeekCompleted;
			this.inputQueue.BeginPeek();
		}
		~MsmqEndpointReceiver()
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

			lock (this.inputQueue)
			{
				if (this.disposed)
					return;

				this.disposed = true;
				this.inputQueue.PeekCompleted -= this.OnPeekCompleted;
				this.inputQueue.Dispose();
			}
		}

		private void OnPeekCompleted(object sender, PeekCompletedEventArgs e)
		{
			var handlers = this.MessageAvailable;
			if (handlers != null)
				handlers(this, EventArgs.Empty);

			this.inputQueue.BeginPeek();
		}

		public virtual PhysicalMessage Receive()
		{
			try
			{
				using (var message = this.inputQueue.Receive(ReceiveMessageTimeout))
					return message.BuildPhysicalMessage(this.serializer);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
					return null;

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(MsmqMessages.AccessDenied, this.inputQueue.QueueName);

				throw new EndpointException(e.Message, e);
			}
		}
	}
}