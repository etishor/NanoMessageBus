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
		public XmlMessageSerializer(Type envelopeMessageType)
		{
			envelopeMessageType = envelopeMessageType ?? typeof(EnvelopeMessage);
			Log.Debug(Diagnostics.DefaultEnvelopeMessage, envelopeMessageType);
			this.serializer = new DataContractSerializer(envelopeMessageType);
		}

		protected override void SerializeMessage(Stream output, object message)
		{
			this.serializer.WriteObject(output, message);
		}
		protected override object DeserializeMessage(Stream input)
		{
			return this.serializer.ReadObject(input);
		}
	}
}