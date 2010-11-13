namespace NanoMessageBus.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class PhysicalMessage
	{
		// TODO: all private properties set in ctor
		public Guid MessageId { get; set; }
		public Guid CorrelationId { get; set; }
		public string ReturnAddress { get; set; }
		public string DestinationAddress { get; set; }
		public DateTime Expiration { get; set; }
		public bool Durable { get; set; }
		public IDictionary<string, string> Headers { get; set; }
		public ICollection LogicalMessages { get; set; }
	}
}