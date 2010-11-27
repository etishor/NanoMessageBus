namespace NanoMessageBus.Serialization
{
	using System.IO;
	using System.IO.Compression;

	public class GzipMessageSerializer : SerializerBase
	{
		private readonly ISerializeMessages inner;

		public GzipMessageSerializer(ISerializeMessages inner)
		{
			this.inner = inner;
		}

		protected override void SerializeMessage(object message, Stream output)
		{
			using (var compressedStream = new DeflateStream(output, CompressionMode.Compress, true))
				this.inner.Serialize(message, compressedStream);
		}

		protected override object DeserializeMessage(Stream input)
		{
			using (var inflatedStream = new DeflateStream(input, CompressionMode.Decompress, true))
				return this.inner.Deserialize(inflatedStream);
		}
	}
}