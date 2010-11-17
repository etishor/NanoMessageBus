namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.IO;
	using System.Messaging;
	using Logging;

	public class MsmqSenderEndpoint : ISendToEndpoints
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqReceiverEndpoint));
		private readonly Func<string, MsmqConnector> connectorFactory;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqSenderEndpoint(Func<string, MsmqConnector> connectorFactory, ISerializeMessages serializer)
		{
			this.connectorFactory = connectorFactory;
			this.serializer = serializer;
		}
		~MsmqSenderEndpoint()
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
			LogMessage(message);
			using (var serializedStream = new MemoryStream())
			{
				this.serializer.Serialize(message, serializedStream);
				this.Send(message.BuildMessage(serializedStream));
			}
		}
		private static void LogMessage(PhysicalMessage message)
		{
			Log.Debug(MsmqMessages.PreparingMessageToSend, message.MessageId, message.LogicalMessages.Count);
			foreach (var logicalMessage in message.LogicalMessages)
				Log.Verbose(MsmqMessages.PhysicalMessageContains, message.MessageId, logicalMessage.GetType().FullName);
		}

		private void Send(Message message, params string[] recipients)
		{
			using (message)
				foreach (var recipient in recipients ?? new string[] { })
					this.Send(recipient, message);
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