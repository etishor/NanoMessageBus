namespace NanoMessageBus.Endpoints.Serialization
{
	using System.IO;
	using System.Security.Cryptography;

	public class EncryptMessageSerializer : ISerializeMessages
	{
		private readonly ISerializeMessages inner;
		private readonly byte[] encryptionKey;

		public EncryptMessageSerializer(ISerializeMessages inner, byte[] encryptionKey)
		{
			this.encryptionKey = encryptionKey;
			this.inner = inner;
		}

		public Stream Serialize(object message)
		{
			using (var rijndael = new RijndaelManaged())
			{
				rijndael.Key = this.encryptionKey;
				rijndael.Mode = CipherMode.CBC;
				rijndael.GenerateIV();

				using (var encryptor = rijndael.CreateEncryptor())
				using (var workingStream = new MemoryStream())
				using (var encryptionStream = new CryptoStream(workingStream, encryptor, CryptoStreamMode.Write))
				using (var serializedStream = this.inner.Serialize(message))
				{
					workingStream.Write(rijndael.IV, 0, rijndael.IV.Length);

					serializedStream.ReadInto(encryptionStream);
					encryptionStream.Flush();
					encryptionStream.FlushFinalBlock();
					workingStream.Flush();
					workingStream.Position = 0;

					return workingStream.ReadInto(new MemoryStream((int)workingStream.Length));
				}
			}
		}

		public object Deserialize(Stream payload)
		{
			using (var rijndael = new RijndaelManaged())
			{
				rijndael.Key = this.encryptionKey;
				rijndael.IV = GetInitVectorFromStream(payload, rijndael.IV.Length);
				rijndael.Mode = CipherMode.CBC;

				using (var decryptor = rijndael.CreateDecryptor())
				using (var decryptedStream = new CryptoStream(payload, decryptor, CryptoStreamMode.Read))
					return this.inner.Deserialize(decryptedStream);
			}
		}
		private static byte[] GetInitVectorFromStream(Stream encrypted, int initVectorSizeInBytes)
		{
			var buffer = new byte[initVectorSizeInBytes];
			encrypted.Read(buffer, 0, buffer.Length);
			return buffer;
		}
	}
}