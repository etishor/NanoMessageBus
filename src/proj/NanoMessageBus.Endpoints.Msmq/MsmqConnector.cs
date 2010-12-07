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
		private readonly bool enlist;
		private readonly MsmqAddress address;
		private bool disposed;

		public static MsmqConnector OpenReceive(MsmqAddress address, bool enlist)
		{
			var queue = new MessageQueue(address.Proprietary, QueueAccessMode.Receive);
			queue.MessageReadPropertyFilter.SetAll();

			Log.Info(Diagnostics.OpeningQueueForReceive, address, enlist);

			if (!enlist || queue.Transactional)
				return new MsmqConnector(queue, address, enlist);

			queue.Dispose();
			Log.Error(Diagnostics.NonTransactionalQueue);
			throw new EndpointException(Diagnostics.NonTransactionalQueue);
		}
		public static MsmqConnector OpenSend(MsmqAddress address, bool enlist)
		{
			var queue = new MessageQueue(address.Proprietary, QueueAccessMode.Send);
			Log.Info(Diagnostics.OpeningQueueForSend, address, enlist);
			return new MsmqConnector(queue, address, enlist);
		}

		private MsmqConnector(MessageQueue queue, MsmqAddress address, bool enlist)
		{
			this.queue = queue;
			this.address = address;
			this.enlist = enlist;
		}
		~MsmqConnector()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed || !disposing)
				return;

			this.disposed = true;

			Log.Debug(Diagnostics.DisposingQueue, this.Address);
			this.queue.Dispose();
		}

		public virtual Uri Address
		{
			get { return this.address.Canonical; }
		}

		public virtual Message Receive(TimeSpan timeout)
		{
			Log.Verbose(Diagnostics.AttemptingToReceiveMessage, this.Address);
			var trx = this.enlist ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None;
			return this.queue.Receive(timeout, trx);
		}

		public virtual void Send(object message)
		{
			Log.Verbose(Diagnostics.SendingMessage, this.Address);
			var trx = (this.enlist && Transaction.Current != null)
				? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.Single;
			this.queue.Send(message, trx);
		}
	}
}