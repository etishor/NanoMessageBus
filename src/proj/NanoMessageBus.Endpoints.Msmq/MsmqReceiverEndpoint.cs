namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Messaging;
	using Logging;
	using Serialization;

	public class MsmqReceiverEndpoint : IReceiveFromEndpoints
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqReceiverEndpoint));
		private static readonly TimeSpan Timeout = 2.Seconds();
		private readonly MsmqConnector connector;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqReceiverEndpoint(MsmqConnector connector, ISerializeMessages serializer)
		{
			this.serializer = serializer;
			this.connector = connector;
		}
		~MsmqReceiverEndpoint()
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

			lock (this.connector)
			{
				if (this.disposed)
					return;

				this.disposed = true;
				this.connector.Dispose();
			}
		}

		public virtual PhysicalMessage Receive()
		{
			try
			{
				var message = this.connector.Receive(Timeout);

				Log.Info(Diagnostics.MessageReceived, message.BodyStream.Length, this.connector.QueueName);

				using (message)
				using (message.BodyStream)
					return message.BuildMessage(this.serializer);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
				{
					Log.Verbose(Diagnostics.NoMessageAvailable, this.connector.QueueName);
					return null;
				}

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(Diagnostics.AccessDenied, this.connector.QueueName);

				throw new EndpointException(e.Message, e);
			}
		}
	}
}