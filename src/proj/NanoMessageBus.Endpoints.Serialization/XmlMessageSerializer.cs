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

		public virtual void Serialize(object message, Stream output)
		{
			this.serializer.WriteObject(output, message);
		}

		public virtual object Deserialize(Stream input)
		{
			return this.serializer.ReadObject(input);
		}
	}
}