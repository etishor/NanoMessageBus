namespace NanoMessageBus.Serialization
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
			using (var compressedStream = new DeflateStream(output, CompressionMode.Compress, true))
				this.inner.Serialize(message, compressedStream);
		}

		public virtual object Deserialize(Stream input)
		{
			Log.Verbose(Diagnostics.Deserializing, input.CanSeek ? (object)input.Length : "unknown");
			using (var inflatedStream = new DeflateStream(input, CompressionMode.Decompress, true))
				return this.inner.Deserialize(inflatedStream);
		}
	}
}