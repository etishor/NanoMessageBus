namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Messaging;
	using System.Runtime.Serialization;
	using Logging;
	using Serialization;

	public class MsmqSenderEndpoint : ISendToEndpoints
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqReceiverEndpoint));
		private const string LabelFormat = "NMB:{0}:{1}";
		private readonly IDictionary<string, MsmqConnector> activeConnectors;
		private readonly Func<string, MsmqConnector> connectorFactory;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqSenderEndpoint(Func<string, MsmqConnector> connectorFactory, ISerializeMessages serializer)
		{
			this.activeConnectors = new Dictionary<string, MsmqConnector>();
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

			lock (this.activeConnectors)
			{
				this.disposed = true;

				foreach (var connector in this.activeConnectors.Values)
					connector.Dispose();

				this.activeConnectors.Clear();
			}
		}

		public virtual void Send(PhysicalMessage message, params string[] recipients)
		{
			Log.Debug(Diagnostics.PreparingMessageToSend, message.MessageId, message.LogicalMessages.Count);
			foreach (var msg in message.LogicalMessages)
				Log.Verbose(Diagnostics.PhysicalMessageContains, message.MessageId, msg.GetType().FullName);

			using (var serializedStream = new MemoryStream())
			{
				this.serializer.Serialize(message, serializedStream);
				this.Send(BuildMsmqMessage(message, serializedStream), recipients);
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
				this.OpenConnector(address).Send(message);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
					Log.Error(Diagnostics.QueueNotFound, address);

				if (e.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
					Log.Fatal(Diagnostics.AccessDenied, address);

				throw new EndpointException(e.Message, e);
			}
		}
		private MsmqConnector OpenConnector(string address)
		{
			MsmqConnector connector;
			if (this.activeConnectors.TryGetValue(address, out connector))
				return connector;

			lock (this.activeConnectors)
			{
				if (this.disposed)
					throw new ObjectDisposedException(Diagnostics.EndpointAlreadyDisposed);

				if (!this.activeConnectors.TryGetValue(address, out connector))
					this.activeConnectors[address] = connector = this.connectorFactory(address);

				return connector;
			}
		}
		public static Message BuildMsmqMessage(PhysicalMessage message, Stream serialized)
		{
			return new Message
			{
				Label = GetLabel(message),
				BodyStream = serialized,
				Recoverable = message.Persistent,
				TimeToBeReceived = GetTimeToBeReceived(message),
			};
		}
		private static string GetLabel(PhysicalMessage message)
		{
			var messages = message.LogicalMessages;
			return LabelFormat.FormatWith(messages.Count, messages.First().GetType().FullName);
		}
		private static TimeSpan GetTimeToBeReceived(PhysicalMessage message)
		{
			if (message.TimeToLive == TimeSpan.MaxValue || message.TimeToLive == TimeSpan.Zero)
				return MessageQueue.InfiniteTimeout;

			return message.TimeToLive;
		}
	}
}