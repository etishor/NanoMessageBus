namespace NanoMessageBus.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization;
	using ProtoBuf;

	public class ProtocolBufferSerializer : SerializerBase
	{
		private readonly Dictionary<int, Type> keys = new Dictionary<int, Type>();
		private readonly Dictionary<Type, int> types = new Dictionary<Type, int>();
		private readonly Dictionary<Type, IFormatter> formatters = new Dictionary<Type, IFormatter>();

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

			var key = messageType.FullName.GetHashCode();
			this.keys.Add(key, messageType);
			this.types.Add(messageType, key);
			this.formatters.Add(messageType, CreateFormatter(messageType));
		}
		private static IFormatter CreateFormatter(Type type)
		{
			typeof(Serializer).GetMethod("PrepareSerializer")
				.MakeGenericMethod(type).Invoke(null, null);

			return (IFormatter)typeof(Serializer).GetMethod("CreateFormatter")
				.MakeGenericMethod(type).Invoke(null, null);
		}

		protected override void SerializeMessage(object message, Stream output)
		{
			int key;
			if (!this.types.TryGetValue(message.GetType(), out key))
				throw new SerializationException(message.GetType() + " has not been registered with the serializer");

			var header = BitConverter.GetBytes(key);
			output.Write(header, 0, header.Length);

			this.formatters[message.GetType()].Serialize(output, message);
		}
		protected override object DeserializeMessage(Stream input)
		{
			var header = new byte[4];
			input.Read(header, 0, header.Length);
			var key = BitConverter.ToInt32(header, 0);

			Type messageType;
			if (!this.keys.TryGetValue(key, out messageType))
				throw new SerializationException(key + " has not been registered with the serializer");

			return this.formatters[messageType].Deserialize(input);
		}
	}
}