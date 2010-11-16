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
					Transfer(serializedStream, compressedStream);

				outputStream.Position = 0;
				return outputStream;
			}
		}
		private static void Transfer(Stream source, Stream destination)
		{
			var buffer = new byte[(int)source.Length];
			int read;
			while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
				destination.Write(buffer, 0, read);
		}

		public virtual object Deserialize(Stream payload)
		{
			var inflatedStream = new DeflateStream(payload, CompressionMode.Decompress);
			return this.inner.Deserialize(inflatedStream);
		}
	}
}