namespace SendReceive
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class MyMessage
	{
		public MyMessage()
		{
			this.MessageId = Guid.NewGuid();
		}

		[DataMember(EmitDefaultValue = false, IsRequired = true, Order = 1)]
		public Guid MessageId { get; set; }
	}
}