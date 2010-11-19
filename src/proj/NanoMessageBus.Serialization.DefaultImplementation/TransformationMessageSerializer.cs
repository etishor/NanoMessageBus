namespace NanoMessageBus.Serialization
{
	using System.IO;

	public class TransformationMessageSerializer : ISerializeMessages
	{
		private readonly ISerializeMessages inner;
		private readonly ITransformMessages transformer;

		public TransformationMessageSerializer(ISerializeMessages inner, ITransformMessages transformer)
		{
			this.transformer = transformer;
			this.inner = inner;
		}

		public void Serialize(object message, Stream output)
		{
			message = this.transformer.Transform(message);
			this.inner.Serialize(message, output);
		}
		public object Deserialize(Stream input)
		{
			var message = this.inner.Deserialize(input);
			return this.transformer.Transform(message);
		}
	}
}