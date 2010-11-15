namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;
	using Logging;

	public class MsmqEndpointSender : ISendToEndpoints
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqEndpointReceiver));
		private readonly Func<string, MsmqConnector> connectorFactory;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqEndpointSender(Func<string, MsmqConnector> connectorFactory, ISerializeMessages serializer)
		{
			this.connectorFactory = connectorFactory;
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
			using (var envelope = message.BuildMessage(this.serializer))
				foreach (var recipient in recipients ?? new string[] {})
					this.Send(recipient, envelope);
		}
		private void Send(string address, object message)
		{
			try
			{
				using (var connector = this.connectorFactory(address))
					connector.Send(message, string.Empty);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
					Log.Fatal(MsmqMessages.QueueNotFound, address);

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(MsmqMessages.AccessDenied, address);

				throw new EndpointException(e.Message, e);
			}
		}
	}
}