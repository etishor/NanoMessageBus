namespace NanoMessageBus.MessageSubscriber
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	[Serializable]
	[DataContract]
	public class SubscriptionRequestMessage
	{
		[DataMember(EmitDefaultValue = false, Name = "Subscriber")]
		public string Subscriber { get; set; }

		[DataMember(EmitDefaultValue = false, Name = "MessageTypes")]
		public ICollection<string> MessageTypes { get; set; }

		[DataMember(EmitDefaultValue = false, Name = "Expiration")]
		public DateTime Expiration { get; set; }
	}
}