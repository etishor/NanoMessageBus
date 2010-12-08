namespace NanoMessageBus.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using ProtoBuf;

	public class ProtocolBufferSerializer : SerializerBase
	{
		private readonly Dictionary<int, Type> keys = new Dictionary<int, Type>();
		private readonly Dictionary<Type, int> types = new Dictionary<Type, int>();

		private readonly Dictionary<Type, Func<Stream, object>> deserializers =
			new Dictionary<Type, Func<Stream, object>>();

		public ProtocolBufferSerializer()
			: this(new Type[] { })
		{
		}
		public ProtocolBufferSerializer(params string[] messageAssemblyFilenamePatterns)
			: this(messageAssemblyFilenamePatterns.LoadAssemblies())
		{
		}
		public ProtocolBufferSerializer(params Assembly[] messageAssemblies)
			: this(messageAssemblies.SelectMany(assembly => assembly.GetTypes()).ToArray())
		{
		}
		public ProtocolBufferSerializer(params Type[] types)
		{
			this.RegisterPrimitives();

			foreach (var type in types ?? new Type[] { })
				this.RegisterType(type);
		}
		private void RegisterPrimitives()
		{
			this.RegisterType(typeof(bool));

			this.RegisterType(typeof(char));
			this.RegisterType(typeof(byte));
			this.RegisterType(typeof(sbyte));

			this.RegisterType(typeof(short));
			this.RegisterType(typeof(ushort));
			this.RegisterType(typeof(int));
			this.RegisterType(typeof(uint));
			this.RegisterType(typeof(long));
			this.RegisterType(typeof(ulong));
			this.RegisterType(typeof(double));
			this.RegisterType(typeof(float));
			this.RegisterType(typeof(decimal));

			this.RegisterType(typeof(string));

			this.RegisterType(typeof(Uri));
			this.RegisterType(typeof(Guid));
			this.RegisterType(typeof(ProtocolBufferTransportMessage));
			this.RegisterType(typeof(Exception));
			this.RegisterType(typeof(SerializationException));
		}
		private void RegisterType(Type type)
		{
			if (type == null || string.IsNullOrEmpty(type.FullName))
				return;

			if (this.types.ContainsKey(type))
				return; // already registered

			var key = type.FullName.GetHashCode();
			this.keys.Add(key, type);
			this.types.Add(type, key);

			// TODO: make this faster by using reflection to create a delegate and then invoking the delegate
			// http://stackoverflow.com/questions/2490828/createdelegate-with-unknown-types/2493903#2493903
			var deserialize = typeof(Serializer).GetMethod("Deserialize").MakeGenericMethod(type);
			this.deserializers[type] = stream => deserialize.Invoke(null, new object[] { stream });
		}

		protected override void SerializeMessage(Stream output, object message)
		{
			var messageType = message.GetType();
			this.WriteTypeToStream(messageType, output);

			if (messageType == typeof(TransportMessage))
				this.SerializeMessage(output, message as TransportMessage);
			else
				Serializer.Serialize(output, message);
		}
		private void WriteTypeToStream(Type messageType, Stream output)
		{
			if (messageType == typeof(TransportMessage))
				messageType = typeof(ProtocolBufferTransportMessage);

			int key;
			if (!this.types.TryGetValue(messageType, out key))
				throw new SerializationException(Diagnostics.UnregisteredType.FormatWith(messageType));

			var header = BitConverter.GetBytes(key);
			output.Write(header, 0, header.Length);
		}
		private void SerializeMessage(Stream output, TransportMessage message)
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

			return messageType == typeof(TransportMessage) ? typeof(ProtocolBufferTransportMessage) : messageType;
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