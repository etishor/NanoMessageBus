namespace NanoMessageBus.Serialization
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using Logging;

	public class XmlMessageSerializer : SerializerBase
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(XmlMessageSerializer));
		private readonly DataContractSerializer serializer;

		public XmlMessageSerializer()
			: this(null)
		{
		}
		public XmlMessageSerializer(Type messageEnvelopeType)
		{
			var envelopeType = messageEnvelopeType ?? typeof(PhysicalMessage);
			Log.Debug(Diagnostics.DefaultEnvelope, envelopeType);
			this.serializer = new DataContractSerializer(envelopeType);
		}

		protected override void SerializeMessage(object message, Stream output)
		{
			this.serializer.WriteObject(output, message);
		}
		protected override object DeserializeMessage(Stream input)
		{
			return this.serializer.ReadObject(input);
		}
	}
}