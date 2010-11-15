namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;
	using Transport;

	public class MsmqAdapter : IDisposable
	{
		private readonly MessageQueue queue;
		private bool disposed;

		public MsmqAdapter(string address, QueueAccessMode accessMode)
		{
			if (string.IsNullOrEmpty(address))
				throw new EndpointException(MsmqMessages.MissingQueueAddress);

			this.queue = new MessageQueue(address.ToQueuePath(), accessMode);

			this.queue.MessageReadPropertyFilter.SetAll();
		}
		~MsmqAdapter()
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
		public virtual bool Transactional
		{
			get { return this.queue.Transactional; }
		}

		public virtual IAsyncResult BeginPeek()
		{
			return this.queue.BeginPeek();
		}
		public virtual event PeekCompletedEventHandler PeekCompleted
		{
			add { this.queue.PeekCompleted += value; }
			remove { this.queue.PeekCompleted -= value; }
		}

		public virtual Message Receive(TimeSpan timeout, MessageQueueTransactionType transactionType)
		{
			return this.queue.Receive(timeout, transactionType);
		}

		public virtual void Send(object message, string label, MessageQueueTransactionType transactionType)
		{
			this.queue.Send(message, label, transactionType);
		}
	}
}