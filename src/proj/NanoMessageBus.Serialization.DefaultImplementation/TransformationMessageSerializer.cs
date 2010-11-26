namespace NanoMessageBus.Serialization
{
	using System.IO;
	using Logging;

	public class TransformationMessageSerializer : ISerializeMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(TransformationMessageSerializer));
		private readonly ISerializeMessages inner;
		private readonly ITransformMessages transformer;

		public TransformationMessageSerializer(ISerializeMessages inner, ITransformMessages transformer)
		{
			this.transformer = transformer;
			this.inner = inner;
		}

		public void Serialize(object message, Stream output)
		{
			Log.Verbose(Diagnostics.Serializing, message.GetType());
			message = this.transformer.Transform(message);
			this.inner.Serialize(message, output);
		}
		public object Deserialize(Stream input)
		{
			Log.Verbose(Diagnostics.Deserializing, input.CanSeek ? (object)input.Length : "unknown");
			var message = this.inner.Deserialize(input);
			return this.transformer.Transform(message);
		}
	}
}