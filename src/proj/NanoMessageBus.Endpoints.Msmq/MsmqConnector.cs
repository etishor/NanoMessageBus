namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;
	using System.Transactions;
	using Logging;

	public class MsmqConnector : IDisposable
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqConnector));
		private readonly MessageQueue queue;
		private readonly MessageQueueTransactionType transactionType;
		private bool disposed;

		public static MsmqConnector OpenReceive(string address, bool enlist)
		{
			var queue = Open(address, QueueAccessMode.Receive);
			queue.MessageReadPropertyFilter.SetAll();

			var transactionType = enlist
				? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None;

			Log.Debug(MsmqMessages.OpeningQueueForReceive, address, transactionType);

			if (!enlist || queue.Transactional)
				return new MsmqConnector(queue, transactionType);

			queue.Dispose();
			Log.Error(MsmqMessages.NonTransactionalQueue);
			throw new EndpointException(MsmqMessages.NonTransactionalQueue);
		}
		public static MsmqConnector OpenSend(string address, bool enlist)
		{
			var queue = Open(address, QueueAccessMode.Send);
			var transactionType = (enlist && Transaction.Current != null)
				? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.Single;

			Log.Debug(MsmqMessages.OpeningQueueForSend, address, transactionType);
			return new MsmqConnector(queue, transactionType);
		}
		private static MessageQueue Open(string address, QueueAccessMode accessMode)
		{
			if (!string.IsNullOrEmpty(address))
				return new MessageQueue(address.ToQueuePath(), accessMode);

			Log.Error(MsmqMessages.MissingQueueAddress);
			throw new EndpointException(MsmqMessages.MissingQueueAddress);
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

				Log.Debug(MsmqMessages.DisposingQueue, this.QueueName);

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
			Log.Verbose(MsmqMessages.AttemptingToReceiveMessage, this.QueueName);
			return this.queue.Receive(timeout, this.transactionType);
		}

		public virtual void Send(object message, string label)
		{
			Log.Verbose(MsmqMessages.SendingMessage, this.QueueName);
			this.queue.Send(message, label, this.transactionType);
		}
	}
}