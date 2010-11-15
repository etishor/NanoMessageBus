namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;
	using System.Transactions;

	public class MsmqProxy : IDisposable
	{
		private readonly MessageQueue queue;
		private readonly MessageQueueTransactionType transactionType;
		private bool disposed;

		public static MsmqProxy OpenRead(string address, bool transactional)
		{
			var queue = Open(address, QueueAccessMode.Receive);
			queue.MessageReadPropertyFilter.SetAll();

			var transactionType = transactional
				? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None;

			if (transactional && !queue.Transactional)
			{
				queue.Dispose();
				throw new EndpointException(MsmqMessages.NonTransactionalQueue);
			}

			return new MsmqProxy(queue, transactionType);
		}
		public static MsmqProxy OpenWrite(string address, bool transactional)
		{
			var queue = Open(address, QueueAccessMode.Send);
			var transactionType = (transactional && Transaction.Current != null)
				? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.Single;

			return new MsmqProxy(queue, transactionType);
		}
		private static MessageQueue Open(string address, QueueAccessMode accessMode)
		{
			if (string.IsNullOrEmpty(address))
				throw new EndpointException(MsmqMessages.MissingQueueAddress);

			return new MessageQueue(address.ToQueuePath(), accessMode);
		}

		private MsmqProxy(MessageQueue queue, MessageQueueTransactionType transactionType)
		{
			this.queue = queue;
			this.transactionType = transactionType;
		}
		~MsmqProxy()
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

			lock (this.queue)
			{
				if (this.disposed)
					return;

				this.disposed = true;
				this.queue.Dispose();
			}
		}

		public virtual string QueueName
		{
			get { return this.queue.QueueName; }
		}

		public virtual Message Receive(TimeSpan timeout)
		{
			return this.queue.Receive(timeout, this.transactionType);
		}

		public virtual void Send(object message, string label)
		{
			this.queue.Send(message, label, this.transactionType);
		}
	}
}