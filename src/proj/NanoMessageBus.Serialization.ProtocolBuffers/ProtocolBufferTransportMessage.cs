namespace NanoMessageBus.Serialization
{
	using System;
	using System.Collections.Generic;
	using ProtoBuf;

	[ProtoContract]
	internal class ProtocolBufferTransportMessage
	{
		public ProtocolBufferTransportMessage()
		{
			this.LogicalMessages = new List<byte[]>();
		}
		public ProtocolBufferTransportMessage(PhysicalMessage message)
			: this()
		{
			this.MessageId = message.MessageId;
			this.ReturnAddress = message.ReturnAddress;
			this.Headers = message.Headers;
		}

		public PhysicalMessage ToMessage()
		{
			return new PhysicalMessage(
				this.MessageId,
				this.ReturnAddress,
				TimeSpan.Zero,
				false,
				this.Headers,
				new List<object>());
		}

		[ProtoMember(1)]
		public Guid MessageId { get; set; }

		[ProtoMember(2)]
		public string ReturnAddress { get; set; }

		[ProtoMember(3)]
		public IDictionary<string, string> Headers { get; set; }

		[ProtoMember(4)]
		public ICollection<byte[]> LogicalMessages { get; set; }
	}
}