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
		private readonly MessageQueueTransactionType transactionType;
		private readonly ISerializeMessages serializer;
		private readonly MsmqAdapter inputQueue;
		private IAsyncResult peekResult;
		private bool disposed;

		public MsmqEndpointReceiver(bool transactional, ISerializeMessages serializer, MsmqAdapter inputQueue)
		{
			this.transactionType = transactional.GetInboundTransactionType();
			this.serializer = serializer;
			this.inputQueue = inputQueue;

			if (transactional && !this.inputQueue.Transactional)
				throw new EndpointException(MsmqMessages.NonTransactionalQueue);

			// http://blogs.msdn.com/b/darioa/archive/2006/09/15/write-your-services-leveraging-existing-thread-pool-technologies.aspx
			this.inputQueue.PeekCompleted += this.OnPeekCompleted;
			this.peekResult = this.inputQueue.BeginPeek();
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
				this.peekResult.AsyncWaitHandle.Dispose();
				this.inputQueue.Dispose();
			}
		}

		private void OnPeekCompleted(object sender, PeekCompletedEventArgs e)
		{
			var handlers = this.MessageAvailable;
			if (handlers != null)
				handlers(this, EventArgs.Empty);

			this.peekResult = this.inputQueue.BeginPeek();
		}

		public virtual PhysicalMessage Receive()
		{
			try
			{
				using (var message = this.inputQueue.Receive(ReceiveMessageTimeout, this.transactionType))
					return message.BuildPhysicalMessage(this.serializer);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
					return this.inputQueue.MessageNoLongerAvailable();

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(MsmqMessages.AccessDenied, this.inputQueue.QueueName);

				throw new EndpointException(e.Message, e);
			}
		}
	}
}