namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Messaging;
	using Logging;
	using Serialization;

	public class MsmqSenderEndpoint : ISendToEndpoints
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqReceiverEndpoint));
		private readonly IDictionary<string, MsmqConnector> activeConnections;
		private readonly Func<string, MsmqConnector> connectorFactory;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqSenderEndpoint(Func<string, MsmqConnector> connectorFactory, ISerializeMessages serializer)
		{
			this.activeConnections = new Dictionary<string, MsmqConnector>();
			this.connectorFactory = connectorFactory;
			this.serializer = serializer;
		}
		~MsmqSenderEndpoint()
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

			lock (this.activeConnections)
			{
				this.disposed = true;

				foreach (var address in this.activeConnections.Keys)
					this.DisposeConnection(address);

				this.activeConnections.Clear();
			}
		}
		private void DisposeConnection(string address)
		{
			MsmqConnector connector;
			if (!this.activeConnections.TryGetValue(address, out connector))
				return;

			connector.Dispose();
			this.activeConnections.Remove(address);
		}

		public virtual void Send(PhysicalMessage message, params string[] recipients)
		{
			Log.Debug(Diagnostics.PreparingMessageToSend, message.MessageId, message.LogicalMessages.Count);
			foreach (var msg in message.LogicalMessages)
				Log.Verbose(Diagnostics.PhysicalMessageContains, message.MessageId, msg.GetType().FullName);

			using (var serializedStream = new MemoryStream())
			{
				this.serializer.Serialize(message, serializedStream);
				this.Send(message.BuildMessage(serializedStream), recipients);
			}
		}
		private void Send(IDisposable message, params string[] recipients)
		{
			using (message)
				foreach (var recipient in recipients ?? new string[] { })
					this.Send(recipient, message);
		}
		private void Send(string address, object message)
		{
			try
			{
				this.GetConnection(address).Send(message);
			}
			catch (MessageQueueException e)
			{
				lock (this.activeConnections)
					this.DisposeConnection(address);

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
					Log.Error(Diagnostics.QueueNotFound, address);

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(Diagnostics.AccessDenied, address);

				throw new EndpointException(e.Message, e);
			}
		}
		private MsmqConnector GetConnection(string address)
		{
			MsmqConnector connector;
			if (this.activeConnections.TryGetValue(address, out connector))
				return connector;

			lock (this.activeConnections)
			{
				if (this.disposed)
					throw new ObjectDisposedException(Diagnostics.EndpointAlreadyDisposed);

				if (!this.activeConnections.TryGetValue(address, out connector))
					this.activeConnections[address] = connector = this.connectorFactory(address);

				return connector;
			}
		}
	}
}