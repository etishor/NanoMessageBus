namespace NanoMessageBus.Serialization
{
	using System;
	using System.Collections.Generic;
	using ProtoBuf;

	[ProtoContract]
	internal class ProtocolBufferEnvelopeMessage
	{
		public ProtocolBufferEnvelopeMessage()
		{
			this.LogicalMessages = new List<byte[]>();
		}
		public ProtocolBufferEnvelopeMessage(EnvelopeMessage message)
			: this()
		{
			this.MessageId = message.MessageId;
			this.ReturnAddress = message.ReturnAddress.ToString();
			this.Headers = message.Headers;
		}

		public EnvelopeMessage ToMessage()
		{
			return new EnvelopeMessage(
				this.MessageId,
				new Uri(this.ReturnAddress),
				TimeSpan.Zero,
				false,
				this.Headers,
				new List<object>());
		}

		[ProtoMember(1)] public Guid MessageId { get; set; }
		[ProtoMember(2)] public string ReturnAddress { get; set; }
		[ProtoMember(3)] public IDictionary<string, string> Headers { get; set; }
		[ProtoMember(4)] public ICollection<byte[]> LogicalMessages { get; set; }
	}
}