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
		public XmlMessageSerializer(Type transportMessageType)
		{
			transportMessageType = transportMessageType ?? typeof(TransportMessage);
			Log.Debug(Diagnostics.DefaultTransportMessage, transportMessageType);
			this.serializer = new DataContractSerializer(transportMessageType);
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