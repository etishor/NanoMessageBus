namespace NanoMessageBus.MessageSubscriber
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represents a request to unsubscribe from types of messages indicated.
	/// </summary>
	[Serializable]
	[DataContract]
	public class UnsubscribeRequestMessage
	{
		/// <summary>
		/// Gets or sets the message types for which a unsubscribe is requested.
		/// </summary>
		[DataMember(EmitDefaultValue = false, Name = "MessageTypes", Order = 1)]
		public ICollection<string> MessageTypes { get; set; }
	}
}