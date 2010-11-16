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

		public virtual Stream Serialize(object message)
		{
			using (var serializedStream = this.inner.Serialize(message))
			{
				var outputStream = new MemoryStream((int)serializedStream.Length);

				using (var compressedStream = new DeflateStream(outputStream, CompressionMode.Compress, true))
					serializedStream.ReadInto(compressedStream);

				outputStream.Position = 0;
				return outputStream;
			}
		}

		public virtual object Deserialize(Stream payload)
		{
			var inflatedStream = new DeflateStream(payload, CompressionMode.Decompress);
			return this.inner.Deserialize(inflatedStream);
		}
	}
}