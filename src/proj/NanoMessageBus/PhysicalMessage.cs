namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class PhysicalMessage
	{
		public PhysicalMessage(
			Guid messageId,
			Guid correlationId,
			string returnAddress,
			DateTime expiration,
			bool durable,
			IDictionary<string, string> headers,
			ICollection<object> logicalMessages)
		{
			this.MessageId = messageId;
			this.CorrelationId = correlationId;
			this.ReturnAddress = returnAddress;
			this.Expiration = expiration;
			this.Durable = durable;
			this.Headers = headers;
			this.LogicalMessages = logicalMessages;
		}

		public Guid MessageId { get; private set; }
		public Guid CorrelationId { get; private set; }
		public string ReturnAddress { get; private set; }
		public DateTime Expiration { get; private set; }
		public bool Durable { get; private set; }
		public IDictionary<string, string> Headers { get; private set; }
		public ICollection<object> LogicalMessages { get; private set; }
	}
}