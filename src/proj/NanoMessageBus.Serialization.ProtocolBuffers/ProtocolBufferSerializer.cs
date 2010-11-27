namespace NanoMessageBus.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using ProtoBuf;

	public class ProtocolBufferSerializer : SerializerBase
	{
		private readonly Dictionary<int, Type> keys = new Dictionary<int, Type>();
		private readonly Dictionary<Type, int> types = new Dictionary<Type, int>();

		private readonly Dictionary<Type, Action<Stream, object>> serializers = 
			new Dictionary<Type, Action<Stream, object>>();
		private readonly Dictionary<Type, Func<Stream, object>> deserializers =
			new Dictionary<Type, Func<Stream, object>>();

		public ProtocolBufferSerializer(params Type[] messageTypes)
		{
			this.RegisterMessage(typeof(PhysicalMessage));

			foreach (var messageType in messageTypes ?? new Type[] { })
				RegisterMessage(messageType);
		}
		private void RegisterMessage(Type messageType)
		{
			if (messageType == null || string.IsNullOrEmpty(messageType.FullName))
				return;

			if (this.types.ContainsKey(messageType))
				return; // already registered

			var key = messageType.FullName.GetHashCode();
			this.keys.Add(key, messageType);
			this.types.Add(messageType, key);

			this.serializers[messageType] = (stream, message) => Serializer.Serialize(stream, message);

			// TODO: make this faster by using reflection to create a delegate and then invoking the delegate
			var deserialize = typeof(Serializer).GetMethod("Deserialize").MakeGenericMethod(messageType);
			this.deserializers[messageType] = stream => deserialize.Invoke(null, new object[] { stream });
		}

		protected override void SerializeMessage(Stream output, object message)
		{
			var messageType = message.GetType();

			int key;
			if (!this.types.TryGetValue(messageType, out key))
				throw new SerializationException(Diagnostics.UnregisteredType.FormatWith(messageType));

			var header = BitConverter.GetBytes(key);
			output.Write(header, 0, header.Length);

			this.serializers[messageType](output, message);
		}
		protected override object DeserializeMessage(Stream input)
		{
			var header = new byte[4];
			input.Read(header, 0, header.Length);
			var key = BitConverter.ToInt32(header, 0);

			Type messageType;
			if (!this.keys.TryGetValue(key, out messageType))
				throw new SerializationException(Diagnostics.UnrecognizedHeader.FormatWith(key));

			return this.deserializers[messageType](input);
		}
	}
}