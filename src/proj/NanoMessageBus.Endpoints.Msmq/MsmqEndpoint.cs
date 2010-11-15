namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;
	using System.Transactions;
	using Core;
	using Logging;
	using Transport;

	public class MsmqEndpoint : IReceiveFromEndpoints, ISendToEndpoints
	{
		public event EventHandler MessageAvailable;

		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqEndpoint));
		private static readonly TimeSpan ReceiveMessageTimeout = 2.Seconds();
		private readonly MessageQueueTransactionType transactionType;
		private readonly ISerializeMessages serializer;
		private readonly MessageQueue inputQueue;
		private bool disposed;

		public MsmqEndpoint(bool transactional, ISerializeMessages serializer, string address)
			: this(transactional, serializer, address.OpenForReceive())
		{
		}
		public MsmqEndpoint(bool transactional, ISerializeMessages serializer, MessageQueue inputQueue)
		{
			this.transactionType = transactional.GetTransactionalType();
			this.serializer = serializer;
			this.inputQueue = inputQueue;

			if (this.inputQueue.Transactional && transactional)
				throw new EndpointException(MsmqMessages.NonTransactionalQueue);

			// http://blogs.msdn.com/b/darioa/archive/2006/09/15/write-your-services-leveraging-existing-thread-pool-technologies.aspx
			this.inputQueue.PeekCompleted += this.OnPeekCompleted;
			this.inputQueue.BeginPeek();
		}

		~MsmqEndpoint()
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
		public virtual void Send(PhysicalMessage message, params string[] recipients)
		{
			var transactional = (Transaction.Current != null)
				? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.Single;

			using (var envelope = message.BuildMsmqMessage(this.serializer))
				foreach (var recipient in recipients ?? new string[0])
					Send(envelope, transactional, recipient);
		}

		private static void Send(Message message, MessageQueueTransactionType transactional, string address)
		{
			using (var outboundQueue = address.OpenForSend())
				outboundQueue.Send(message, transactional);
		}
	}
}