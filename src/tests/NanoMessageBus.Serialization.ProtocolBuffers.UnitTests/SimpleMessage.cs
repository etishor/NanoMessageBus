namespace NanoMessageBus.Serialization.ProtocolBuffers.UnitTests
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	[DataContract]
	internal class SimpleMessage
	{
		[DataMember(Order = 1)]
		public string Value { get; set; }
	}
}