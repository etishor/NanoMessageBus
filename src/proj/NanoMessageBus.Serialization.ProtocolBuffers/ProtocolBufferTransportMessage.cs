namespace NanoMessageBus.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ProtoBuf;

	[ProtoContract]
	internal class ProtocolBufferTransportMessage
	{
		public ProtocolBufferTransportMessage()
		{
			this.LogicalMessages = new List<object>();
		}
		public ProtocolBufferTransportMessage(PhysicalMessage message)
		{
			this.MessageId = message.MessageId;
			this.ReturnAddress = message.ReturnAddress;
			this.Headers = message.Headers;
			this.LogicalMessages = message.LogicalMessages;
		}

		public PhysicalMessage ToMessage()
		{
			return new PhysicalMessage(
				this.MessageId,
				this.ReturnAddress,
				TimeSpan.Zero,
				false,
				this.Headers,
				this.LogicalMessages.ToArray());
		}

		[ProtoMember(1)]
		public Guid MessageId { get; set; }

		[ProtoMember(2)]
		public string ReturnAddress { get; set; }

		[ProtoMember(3)]
		public IDictionary<string, string> Headers { get; set; }

		[ProtoIgnore]
		public ICollection<object> LogicalMessages { get; set; }
	}
}