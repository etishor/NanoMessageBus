namespace NanoMessageBus.MessageSubscriber
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represents a request to subscribe to the types of messages indicated.
	/// </summary>
	[Serializable]
	[DataContract]
	public class SubscriptionRequestMessage
	{
		/// <summary>
		/// Gets or sets the message types for which a subscription is requested.
		/// </summary>
		[DataMember(EmitDefaultValue = false, Name = "MessageTypes", Order = 1)]
		public ICollection<string> MessageTypes { get; set; }

		/// <summary>
		/// Gets or sets the proposed expiration of the subscription, if accepted.
		/// </summary>
		[DataMember(EmitDefaultValue = false, Name = "Expiration", Order = 2)]
		public DateTime? Expiration { get; set; }
	}
}