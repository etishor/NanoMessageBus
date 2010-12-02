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

		private readonly Dictionary<Type, Func<Stream, object>> deserializers =
			new Dictionary<Type, Func<Stream, object>>();

		public ProtocolBufferSerializer(params Type[] messageTypes)
		{
			this.RegisterPrimitives();
			this.RegisterCommonTypes();

			foreach (var messageType in messageTypes ?? new Type[] { })
				this.RegisterMessage(messageType);
		}
		private void RegisterPrimitives()
		{
			this.RegisterMessage(typeof(bool));

			this.RegisterMessage(typeof(char));
			this.RegisterMessage(typeof(byte));
			this.RegisterMessage(typeof(sbyte));

			this.RegisterMessage(typeof(short));
			this.RegisterMessage(typeof(ushort));
			this.RegisterMessage(typeof(int));
			this.RegisterMessage(typeof(uint));
			this.RegisterMessage(typeof(long));
			this.RegisterMessage(typeof(ulong));
			this.RegisterMessage(typeof(double));
			this.RegisterMessage(typeof(float));
			this.RegisterMessage(typeof(decimal));

			this.RegisterMessage(typeof(string));
		}
		private void RegisterCommonTypes()
		{
			this.RegisterMessage(typeof(Uri));
			this.RegisterMessage(typeof(Guid));
			this.RegisterMessage(typeof(ProtocolBufferTransportMessage));
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

			// TODO: make this faster by using reflection to create a delegate and then invoking the delegate
			var deserialize = typeof(Serializer).GetMethod("Deserialize").MakeGenericMethod(messageType);
			this.deserializers[messageType] = stream => deserialize.Invoke(null, new object[] { stream });
		}

		protected override void SerializeMessage(Stream output, object message)
		{
			var messageType = message.GetType();
			this.WriteTypeToStream(messageType, output);

			if (messageType == typeof(PhysicalMessage))
				this.SerializeMessage(output, message as PhysicalMessage);
			else
				Serializer.Serialize(output, message);
		}
		private void WriteTypeToStream(Type messageType, Stream output)
		{
			if (messageType == typeof(PhysicalMessage))
				messageType = typeof(ProtocolBufferTransportMessage);

			int key;
			if (!this.types.TryGetValue(messageType, out key))
				throw new SerializationException(Diagnostics.UnregisteredType.FormatWith(messageType));

			var header = BitConverter.GetBytes(key);
			output.Write(header, 0, header.Length);
		}
		private void SerializeMessage(Stream output, PhysicalMessage message)
		{
			var protoMessage = new ProtocolBufferTransportMessage(message);
			foreach (var logicalMessage in message.LogicalMessages)
			{
				using (var stream = new MemoryStream())
				{
					this.SerializeMessage(stream, logicalMessage);
					protoMessage.LogicalMessages.Add(stream.ToArray());
				}
			}

			Serializer.Serialize(output, protoMessage);
		}

		protected override object DeserializeMessage(Stream input)
		{
			var header = new byte[4];
			input.Read(header, 0, header.Length);
			var messageType = this.GetSerializedType(header);

			var message = this.deserializers[messageType](input);
			if (messageType != typeof(ProtocolBufferTransportMessage))
				return message;

			return this.DeserializeTransportMessage(message as ProtocolBufferTransportMessage);
		}
		private Type GetSerializedType(byte[] header)
		{
			var key = BitConverter.ToInt32(header, 0);

			Type messageType;
			if (!this.keys.TryGetValue(key, out messageType))
				throw new SerializationException(Diagnostics.UnrecognizedHeader.FormatWith(key));

			return messageType == typeof(PhysicalMessage) ? typeof(ProtocolBufferTransportMessage) : messageType;
		}
		private object DeserializeTransportMessage(ProtocolBufferTransportMessage message)
		{
			var transportMessage = message.ToMessage();

			foreach (var serializedLogicalMessage in message.LogicalMessages)
				using (var stream = new MemoryStream(serializedLogicalMessage))
					transportMessage.LogicalMessages.Add(this.DeserializeMessage(stream));

			return transportMessage;
		}
	}
}