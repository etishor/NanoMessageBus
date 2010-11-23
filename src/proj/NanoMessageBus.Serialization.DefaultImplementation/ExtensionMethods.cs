namespace NanoMessageBus.Serialization
{
	using System.IO;

	internal static class ExtensionMethods
	{
		private const int MaxBuffer = 16384;

		public static Stream ReadInto(this Stream source, Stream destination)
		{
			var bufferSize = source.Length > MaxBuffer ? MaxBuffer : (int)source.Length;
			var buffer = new byte[bufferSize];

			int read;
			while ((read = source.Read(buffer, 0, bufferSize)) > 0)
				destination.Write(buffer, 0, read);

			return destination;
		}
		public static bool KeyIsValid(this byte[] key, int length)
		{
			return key != null && key.Length == length;
		}
	}
}