namespace NanoMessageBus.Endpoints.Serialization
{
	using System.IO;
	using System.IO.Compression;

	public class CompressionSerializer : ISerializeMessages
	{
		private readonly ISerializeMessages inner;

		public CompressionSerializer(ISerializeMessages inner)
		{
			this.inner = inner;
		}

		public virtual Stream Serialize(object message)
		{
			return new DeflateStream(this.inner.Serialize(message), CompressionMode.Compress);
		}
		public virtual object Deserialize(Stream payload)
		{
			return this.inner.Deserialize(new DeflateStream(payload, CompressionMode.Decompress));
		}
	}
}