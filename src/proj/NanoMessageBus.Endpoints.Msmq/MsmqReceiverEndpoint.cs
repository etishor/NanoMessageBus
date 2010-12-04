namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Messaging;
	using Logging;
	using Serialization;

	public class MsmqReceiverEndpoint : IReceiveFromEndpoints
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqReceiverEndpoint));
		private static readonly TimeSpan Timeout = 500.Milliseconds();
		private readonly MsmqConnector connector;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqReceiverEndpoint(MsmqConnector connector, ISerializeMessages serializer)
		{
			this.connector = connector;
			this.serializer = serializer;
		}
		~MsmqReceiverEndpoint()
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
			this.connector.Dispose();
		}

		public string EndpointAddress
		{
			get { return this.connector.Address; }
		}

		public virtual TransportMessage Receive()
		{
			var message = this.ReceiveMessage();
			if (message == null)
				return this.NoMessageAvailable();

			Log.Info(Diagnostics.MessageReceived, message.BodyStream.Length, this.connector.Address);

			using (message)
			using (message.BodyStream)
				return (TransportMessage)this.serializer.Deserialize(message.BodyStream);
		}
		private Message ReceiveMessage()
		{
			try
			{
				return this.connector.Receive(Timeout);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
					return null;

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(Diagnostics.AccessDenied, this.connector.Address);

				throw new EndpointException(e.Message, e);
			}
		}
		private TransportMessage NoMessageAvailable()
		{
			Log.Verbose(Diagnostics.NoMessageAvailable, this.connector.Address);
			return null;
		}
	}
}