namespace NanoMessageBus.Queues.Msmq
{
	using System;
	using System.Messaging;
	using Core;
	using Logging;
	using Transport;

	public class MsmqAdapter : IMessageQueue
	{
		public event EventHandler MessageAvailable;

		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqAdapter));
		private static readonly TimeSpan ReceiveMessageTimeout = 1.Seconds();
		private readonly MessageQueueTransactionType transactionType;
		private readonly ISerializeMessages serializer;
		private readonly MessageQueue queue;
		private bool disposed;

		public MsmqAdapter(bool transactional, ISerializeMessages serializer, string address)
			: this(transactional, serializer, address.OpenConnection())
		{
		}
		public MsmqAdapter(bool transactional, ISerializeMessages serializer, MessageQueue queue)
		{
			this.transactionType = transactional.GetTransactionalType();
			this.serializer = serializer;
			this.queue = queue;

			if (this.queue.Transactional && transactional)
				throw new MessagingException(MsmqMessages.NonTransactionalQueue);

			// TODO: investigate threading capabilities of MessageQueue BCL
			// this starts the queue listening for messages
			// anytime a message appears on the queue we raise an event which lets
			// the transport know a message is available
			// TODO: investigate race conditions, e.g. peek completed may happen before anyone is listening
			this.queue.PeekCompleted += this.OnPeekCompleted;
			this.queue.BeginPeek();
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

			lock (this.queue)
			{
				if (this.disposed)
					return;

				this.disposed = true;
				this.queue.PeekCompleted -= this.OnPeekCompleted;
				this.queue.Dispose();
			}
		}

		private void OnPeekCompleted(object sender, PeekCompletedEventArgs e)
		{
			// good stuff
			// http://blogs.msdn.com/b/darioa/archive/2006/09/15/write-your-services-leveraging-existing-thread-pool-technologies.aspx
			var handlers = this.MessageAvailable;
			if (handlers != null)
				handlers(this, EventArgs.Empty);

			this.queue.BeginPeek();
		}

		public PhysicalMessage Dequeue()
		{
			try
			{
				using (var message = this.queue.Receive(ReceiveMessageTimeout, this.transactionType))
					return message.BuildPhysicalMessage(this.queue, this.serializer);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
					return this.queue.MessageNoLongerAvailable();

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(MsmqMessages.AccessDenied, this.queue.QueueName);

				throw new MessagingException(e.Message, e);
			}
		}
		public void Enqueue(PhysicalMessage message)
		{
			// TODO
			// note: send is thread safe, but only when sending an object of type System.Messaging.Message.
		}
	}
}