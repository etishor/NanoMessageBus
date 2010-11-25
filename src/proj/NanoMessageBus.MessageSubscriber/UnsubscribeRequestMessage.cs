namespace NanoMessageBus.MessageSubscriber
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	[Serializable]
	[DataContract]
	public class UnsubscribeRequestMessage
	{
		[DataMember(EmitDefaultValue = false, Name = "MessageTypes")]
		public ICollection<string> MessageTypes { get; set; }
	}
}