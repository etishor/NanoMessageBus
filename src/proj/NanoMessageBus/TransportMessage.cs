namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// The primary message envelope used to hold the metadata and payload necessary to route the message to all
	/// intended recipients.
	/// </summary>
	[DataContract]
	[Serializable]
	public class TransportMessage
	{
		[DataMember(Order = 1, EmitDefaultValue = false, IsRequired = false)]
		private readonly Guid messageId;

		[DataMember(Order = 2, EmitDefaultValue = false, IsRequired = false)]
		private readonly string returnAddress;

		[IgnoreDataMember]
		private readonly TimeSpan timeToLive;

		[IgnoreDataMember]
		private readonly bool persistent;

		[DataMember(Order = 3, EmitDefaultValue = false, IsRequired = false)]
		private readonly IDictionary<string, string> headers;

		[DataMember(Order = 4, EmitDefaultValue = false, IsRequired = false)]
		private readonly ICollection<object> logicalMessages;

		/// <summary>
		/// Initializes a new instance of the TransportMessage class.
		/// </summary>
		protected TransportMessage()
		{
		}

		/// <summary>
		/// Initializes a new instance of the TransportMessage class.
		/// </summary>
		/// <param name="messageId">The value which uniquely identifies the transport message.</param>
		/// <param name="returnAddress">The address to which all replies should be directed.</param>
		/// <param name="timeToLive">The maximum amount of time the message will live prior to successful receipt.</param>
		/// <param name="persistent">A value indicating whether the message is durably stored.</param>
		/// <param name="headers">The message headers which contain additional metadata about the logical messages.</param>
		/// <param name="logicalMessages">The collection of dispatched logical messages.</param>
		public TransportMessage(
			Guid messageId,
			string returnAddress,
			TimeSpan timeToLive,
			bool persistent,
			IDictionary<string, string> headers,
			ICollection<object> logicalMessages)
		{
			this.messageId = messageId;
			this.returnAddress = returnAddress;
			this.timeToLive = timeToLive;
			this.persistent = persistent;
			this.headers = headers ?? new Dictionary<string, string>();
			this.logicalMessages = logicalMessages;
		}

		/// <summary>
		/// Gets the value which uniquely identifies the transport message.
		/// </summary>
		public Guid MessageId
		{
			get { return this.messageId; }
		}

		/// <summary>
		/// Gets the address to which all replies should be directed.
		/// </summary>
		public string ReturnAddress
		{
			get { return this.returnAddress; }
		}

		/// <summary>
		/// Gets the maximum amount of time the message will live prior to successful receipt.
		/// </summary>
		public TimeSpan TimeToLive
		{
			get { return this.timeToLive; }
		}

		/// <summary>
		/// Gets a value indicating whether the message is durably stored.
		/// </summary>
		public bool Persistent
		{
			get { return this.persistent; }
		}

		/// <summary>
		/// Gets the message headers which contain additional metadata about the logical messages.
		/// </summary>
		public IDictionary<string, string> Headers
		{
			get { return this.headers; }
		}

		/// <summary>
		/// Gets the collection of dispatched logical messages.
		/// </summary>
		public ICollection<object> LogicalMessages
		{
			get { return this.logicalMessages; }
		}
	}
}