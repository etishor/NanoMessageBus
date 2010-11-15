namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;
	using System.Transactions;
	using Logging;

	public class MsmqEndpointSender : ISendToEndpoints
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqEndpointReceiver));
		private readonly Func<string, MsmqAdapter> queueFactory;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqEndpointSender(Func<string, MsmqAdapter> queueFactory, ISerializeMessages serializer)
		{
			this.queueFactory = queueFactory;
			this.serializer = serializer;
		}
		~MsmqEndpointSender()
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
		}

		public virtual void Send(PhysicalMessage message, params string[] recipients)
		{
			var transactional = (Transaction.Current != null).GetOutboundTransactionType();
			using (var envelope = message.BuildMsmqMessage(this.serializer))
				foreach (var recipient in recipients ?? new string[0])
					this.Send(recipient, envelope, transactional);
		}
		private void Send(string address, object message, MessageQueueTransactionType transactional)
		{
			try
			{
				using (var outboundQueue = this.queueFactory(address))
					outboundQueue.Send(message, string.Empty, transactional);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(MsmqMessages.AccessDenied, address.ToQueuePath());

				throw;
			}
		}
	}
}