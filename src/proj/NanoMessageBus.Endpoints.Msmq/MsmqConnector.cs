namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;
	using System.Transactions;

	public class MsmqConnector : IDisposable
	{
		private readonly MessageQueue queue;
		private readonly MessageQueueTransactionType transactionType;
		private bool disposed;

		public static MsmqConnector OpenRead(string address, bool transactional)
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

			return new MsmqConnector(queue, transactionType);
		}
		public static MsmqConnector OpenWrite(string address, bool transactional)
		{
			var queue = Open(address, QueueAccessMode.Send);
			var transactionType = (transactional && Transaction.Current != null)
				? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.Single;

			return new MsmqConnector(queue, transactionType);
		}
		private static MessageQueue Open(string address, QueueAccessMode accessMode)
		{
			if (string.IsNullOrEmpty(address))
				throw new EndpointException(MsmqMessages.MissingQueueAddress);

			return new MessageQueue(address.ToQueuePath(), accessMode);
		}

		private MsmqConnector(MessageQueue queue, MessageQueueTransactionType transactionType)
		{
			this.queue = queue;
			this.transactionType = transactionType;
		}
		~MsmqConnector()
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