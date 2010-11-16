namespace NanoMessageBus.Endpoints.Serialization
{
	using System.IO;
	using System.IO.Compression;

	public class GzipMessageSerializer : ISerializeMessages
	{
		private readonly ISerializeMessages inner;

		public GzipMessageSerializer(ISerializeMessages inner)
		{
			this.inner = inner;
		}

		public virtual void Serialize(object message, Stream output)
		{
			using (var serializedStream = new MemoryStream())
			{
				this.inner.Serialize(message, serializedStream);
				using (var compressedStream = new DeflateStream(output, CompressionMode.Compress, true))
					serializedStream.ReadInto(compressedStream);
			}
		}

		public virtual object Deserialize(Stream input)
		{
			var inflatedStream = new DeflateStream(input, CompressionMode.Decompress);
			return this.inner.Deserialize(inflatedStream);
		}
	}
}