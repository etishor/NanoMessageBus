namespace NanoMessageBus.Serialization
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Security.Cryptography;

	public class EncryptMessageSerializer : SerializerBase
	{
		private const int KeyLength = 16; // bytes
		private readonly ISerializeMessages inner;
		private readonly byte[] encryptionKey;

		public EncryptMessageSerializer(ISerializeMessages inner, byte[] encryptionKey)
		{
			if (!KeyIsValid(encryptionKey, KeyLength))
				throw new ArgumentException(Diagnostics.InvalidEncryptionKey, "encryptionKey");

			this.encryptionKey = encryptionKey;
			this.inner = inner;
		}
		private static bool KeyIsValid(ICollection key, int length)
		{
			return key != null && key.Count == length;
		}

		protected override void SerializeMessage(object message, Stream output)
		{
			using (var rijndael = new RijndaelManaged())
			{
				rijndael.Key = this.encryptionKey;
				rijndael.Mode = CipherMode.CBC;
				rijndael.GenerateIV();

				using (var encryptor = rijndael.CreateEncryptor())
				using (var openStream = new DoNotDisposeStream(output))
				using (var encryptionStream = new CryptoStream(openStream, encryptor, CryptoStreamMode.Write))
				{
					openStream.Write(rijndael.IV, 0, rijndael.IV.Length);
					this.inner.Serialize(message, encryptionStream);
					encryptionStream.Flush();
					encryptionStream.FlushFinalBlock();
				}
			}
		}

		protected override object DeserializeMessage(Stream input)
		{
			using (var rijndael = new RijndaelManaged())
			{
				rijndael.Key = this.encryptionKey;
				rijndael.IV = GetInitVectorFromStream(input, rijndael.IV.Length);
				rijndael.Mode = CipherMode.CBC;

				using (var decryptor = rijndael.CreateDecryptor())
				using (var decryptedStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
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