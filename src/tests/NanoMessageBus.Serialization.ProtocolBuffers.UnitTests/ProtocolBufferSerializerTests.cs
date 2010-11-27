#pragma warning disable 169
// ReSharper disable InconsistentNaming

namespace NanoMessageBus.Serialization.ProtocolBuffers.UnitTests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization;
	using Machine.Specifications;

	[Subject("ProtocolBufferSerializer")]
	public class when_serializing_an_object
	{
		const int InputValue = 1234;
		static readonly Stream OutputStream = new MemoryStream();
		static readonly ISerializeMessages Serializer = new ProtocolBufferSerializer(InputValue.GetType());

		Because of = () => Serializer.Serialize(OutputStream, 0);

		It should_write_the_serialized_bytes_to_the_output_stream_provided = () =>
			OutputStream.Length.ShouldBeGreaterThan(0);
	}

	[Subject("ProtocolBufferSerializer")]
	public class when_serializing_an_unregistered_type
	{
		static readonly ISerializeMessages Serializer = new ProtocolBufferSerializer();
		static Exception Exception;

		Because of = () => Exception = Catch.Exception(() => Serializer.Serialize(null, 0));

		It should_throw_a_SerializationException = () =>
			Exception.ShouldBeOfType(typeof(SerializationException));
	}

	[Subject("ProtocolBufferSerializer")]
	public class when_serializing_and_then_deserializing_a_primitive_type
	{
		const int InputValue = 12345;
		static readonly Stream TempStream = new MemoryStream();
		static readonly ISerializeMessages Serializer = new ProtocolBufferSerializer(InputValue.GetType());
		static int outputValue;

		Establish context = () =>
		{
			Serializer.Serialize(TempStream, InputValue);
			TempStream.Position = 0;
		};

		Because of = () =>
			outputValue = (int)Serializer.Deserialize(TempStream);

		It should_deserialize_the_primitive_type_correctly = () =>
			outputValue.ShouldEqual(InputValue);
	}

	[Subject("ProtocolBufferSerializer")]
	public class when_serializing_and_then_deserializing_a_complex_type
	{
		static readonly Dictionary<string, string> InputValue = new Dictionary<string, string>();
		static readonly Stream TempStream = new MemoryStream();
		static readonly ISerializeMessages Serializer = new ProtocolBufferSerializer(InputValue.GetType());
		static Dictionary<string, string> outputValue;

		Establish context = () =>
		{
			InputValue["a"] = "1";
			InputValue["b"] = "2";

			Serializer.Serialize(TempStream, InputValue);
			TempStream.Position = 0;
		};

		Because of = () =>
			outputValue = (Dictionary<string, string>)Serializer.Deserialize(TempStream);

		It should_deserialize_back_to_the_same_type = () =>
			outputValue.ShouldBeOfType(InputValue.GetType());

		It should_deserialize_the_contents_of_the_complex_type = () =>
			outputValue.Keys.Count.ShouldEqual(InputValue.Keys.Count);
	}

	[Subject("ProtocolBufferSerializer")]
	public class when_serializing_and_then_deserializing_a_PhysicalMessage
	{
		static readonly PhysicalMessage InputValue =
			new PhysicalMessage(Guid.NewGuid(), "ReturnAddress", TimeSpan.Zero, true, null, null);
		static readonly Stream TempStream = new MemoryStream();
		static readonly ISerializeMessages Serializer = new ProtocolBufferSerializer(InputValue.GetType());
		static PhysicalMessage outputValue;

		Establish context = () =>
		{
			Serializer.Serialize(TempStream, InputValue);
			TempStream.Position = 0;
		};

		Because of = () =>
			outputValue = (PhysicalMessage)Serializer.Deserialize(TempStream);

		It should_deserialize_back_to_the_same_type = () =>
			outputValue.ShouldBeOfType(InputValue.GetType());

		It should_deserialize_the_contents_of_the_complex_type = () =>
			outputValue.MessageId.ShouldEqual(InputValue.MessageId);
	}
}

// ReSharper restore InconsistentNaming
#pragma warning restore 169