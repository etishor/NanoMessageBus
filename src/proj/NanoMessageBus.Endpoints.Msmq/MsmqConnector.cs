namespace NanoMessageBus.Endpoints
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

			Log.Info(Diagnostics.OpeningQueueForReceive, address, transactionType);

			if (!enlist || queue.Transactional)
				return new MsmqConnector(queue, transactionType);

			queue.Dispose();
			Log.Error(Diagnostics.NonTransactionalQueue);
			throw new EndpointException(Diagnostics.NonTransactionalQueue);
		}
		public static MsmqConnector OpenSend(string address, bool enlist)
		{
			var queue = Open(address, QueueAccessMode.Send);
			var transactionType = (enlist && Transaction.Current != null)
				? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.Single;

			Log.Info(Diagnostics.OpeningQueueForSend, address, transactionType);
			return new MsmqConnector(queue, transactionType);
		}
		private static MessageQueue Open(string address, QueueAccessMode accessMode)
		{
			if (!string.IsNullOrEmpty(address))
				return new MessageQueue(address.ToQueuePath(), accessMode);

			Log.Error(Diagnostics.MissingQueueAddress);
			throw new EndpointException(Diagnostics.MissingQueueAddress);
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

			this.disposed = true;

			Log.Debug(Diagnostics.DisposingQueue, this.QueueName);
			this.queue.Dispose();
		}

		public virtual string QueueName
		{
			get { return this.queue.QueueName; }
		}

		public virtual Message Receive(TimeSpan timeout)
		{
			Log.Verbose(Diagnostics.AttemptingToReceiveMessage, this.QueueName);
			return this.queue.Receive(timeout, this.transactionType);
		}

		public virtual void Send(object message)
		{
			Log.Verbose(Diagnostics.SendingMessage, this.QueueName);
			this.queue.Send(message, this.transactionType);
		}
	}
}