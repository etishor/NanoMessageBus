namespace NanoMessageBus.Endpoints.Serialization
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;

	public class XmlMessageSerializer : ISerializeMessages
	{
		private readonly DataContractSerializer serializer;

		public XmlMessageSerializer()
			: this(null)
		{
		}
		public XmlMessageSerializer(Type messageEnvelopeType)
		{
			this.serializer = new DataContractSerializer(messageEnvelopeType ?? typeof(PhysicalMessage));
		}

		public virtual Stream Serialize(object message)
		{
			var stream = new MemoryStream();
			this.serializer.WriteObject(stream, message);
			stream.Position = 0;
			return stream;
		}

		public virtual object Deserialize(Stream payload)
		{
			return this.serializer.ReadObject(payload);
		}
	}
}