namespace NanoMessageBus.Serialization
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using Logging;

	public class XmlMessageSerializer : ISerializeMessages
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

		public virtual void Serialize(object message, Stream output)
		{
			Log.Verbose(Diagnostics.Serializing, message.GetType());
			this.serializer.WriteObject(output, message);
		}
		public virtual object Deserialize(Stream input)
		{
			Log.Verbose(Diagnostics.Deserializing, input.Length);
			return this.serializer.ReadObject(input);
		}
	}
}