namespace NanoMessageBus.Serialization
{
	using System.IO;

	public class TransformationMessageSerializer : SerializerBase
	{
		private readonly ISerializeMessages inner;
		private readonly ITransformMessages transformer;

		public TransformationMessageSerializer(ISerializeMessages inner, ITransformMessages transformer)
		{
			this.transformer = transformer;
			this.inner = inner;
		}

		protected override void SerializeMessage(object message, Stream output)
		{
			message = this.transformer.Transform(message);
			this.inner.Serialize(message, output);
		}
		protected override object DeserializeMessage(Stream input)
		{
			var message = this.inner.Deserialize(input);
			return this.transformer.Transform(message);
		}
	}
}