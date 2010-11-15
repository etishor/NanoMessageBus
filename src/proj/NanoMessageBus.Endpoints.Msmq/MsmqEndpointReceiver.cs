namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;
	using Logging;

	public class MsmqEndpointReceiver : IReceiveFromEndpoints
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqEndpointReceiver));
		private static readonly TimeSpan Timeout = 2.Seconds();
		private readonly MsmqProxy inputQueue;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqEndpointReceiver(MsmqProxy inputQueue, ISerializeMessages serializer)
		{
			this.serializer = serializer;
			this.inputQueue = inputQueue;
		}
		~MsmqEndpointReceiver()
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
				this.inputQueue.Dispose();
			}
		}

		public virtual PhysicalMessage Receive()
		{
			try
			{
				using (var message = this.inputQueue.Receive(Timeout))
					return message.BuildPhysicalMessage(this.serializer);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
					return null;

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(MsmqMessages.AccessDenied, this.inputQueue.QueueName);

				throw new EndpointException(e.Message, e);
			}
		}
	}
}