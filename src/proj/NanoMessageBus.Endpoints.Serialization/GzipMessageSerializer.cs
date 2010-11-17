namespace NanoMessageBus.Endpoints.Serialization
{
	using System.IO;
	using System.IO.Compression;
	using Logging;

	public class GzipMessageSerializer : ISerializeMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(GzipMessageSerializer));
		private readonly ISerializeMessages inner;

		public GzipMessageSerializer(ISerializeMessages inner)
		{
			this.inner = inner;
		}

		public virtual void Serialize(object message, Stream output)
		{
			Log.Verbose(Diagnostics.Serializing, message.GetType());
			using (var serializedStream = new MemoryStream())
			{
				this.inner.Serialize(message, serializedStream);
				using (var compressedStream = new DeflateStream(output, CompressionMode.Compress, true))
					serializedStream.ReadInto(compressedStream);
			}
		}

		public virtual object Deserialize(Stream input)
		{
			Log.Verbose(Diagnostics.Deserializing, input.Length);
			var inflatedStream = new DeflateStream(input, CompressionMode.Decompress);
			return this.inner.Deserialize(inflatedStream);
		}
	}
}