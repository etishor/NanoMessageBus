namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Messaging;
	using Logging;
	using Serialization;

	public class MsmqSenderEndpoint : ISendToEndpoints
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MsmqReceiverEndpoint));
		private const string LabelFormat = "NMB:{0}:{1}";
		private readonly IDictionary<Uri, MsmqConnector> activeConnectors = new Dictionary<Uri, MsmqConnector>();
		private readonly Func<Uri, MsmqConnector> connectorFactory;
		private readonly ISerializeMessages serializer;
		private bool disposed;

		public MsmqSenderEndpoint(Func<Uri, MsmqConnector> connectorFactory, ISerializeMessages serializer)
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

		public virtual void Send(EnvelopeMessage message, params Uri[] recipients)
		{
			Log.Debug(Diagnostics.PreparingMessageToSend, message.MessageId, message.LogicalMessages.Count);
			foreach (var msg in message.LogicalMessages)
				Log.Verbose(Diagnostics.EnvelopeMessageContains, message.MessageId, msg.GetType().FullName);

			using (var serializedStream = new MemoryStream())
			{
				this.serializer.Serialize(serializedStream, message);
				this.Send(BuildMsmqMessage(message, serializedStream), recipients);
			}
		}

		private void Send(IDisposable message, params Uri[] recipients)
		{
			using (message)
				foreach (var recipient in recipients ?? new Uri[] { })
					this.Send(recipient, message);
		}
		private void Send(Uri address, object message)
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
		private MsmqConnector OpenConnector(Uri address)
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
		public static Message BuildMsmqMessage(EnvelopeMessage message, Stream serialized)
		{
			return new Message
			{
				Label = GetLabel(message),
				BodyStream = serialized,
				Recoverable = message.Persistent,
				TimeToBeReceived = GetTimeToBeReceived(message),
			};
		}
		private static string GetLabel(EnvelopeMessage message)
		{
			var messages = message.LogicalMessages;
			return LabelFormat.FormatWith(messages.Count, messages.First().GetType().FullName);
		}
		private static TimeSpan GetTimeToBeReceived(EnvelopeMessage message)
		{
			if (message.TimeToLive == TimeSpan.MaxValue || message.TimeToLive == TimeSpan.Zero)
				return MessageQueue.InfiniteTimeout;

			return message.TimeToLive;
		}
	}
}