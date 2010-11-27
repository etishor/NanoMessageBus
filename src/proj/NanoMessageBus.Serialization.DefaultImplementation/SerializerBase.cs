namespace NanoMessageBus.Serialization
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using Logging;

	public abstract class SerializerBase : ISerializeMessages
	{
		private readonly ILog log;

		protected SerializerBase()
		{
			this.log = LogFactory.BuildLogger(this.GetType());
		}

		public void Serialize(Stream output, object message)
		{
			try
			{
				this.log.Verbose(Diagnostics.Serializing, message.GetType());
				this.SerializeMessage(output, message);
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception e)
			{
				this.log.Error(Diagnostics.SerializationFailed);
				throw new SerializationException(Diagnostics.SerializationFailed, e);
			}
		}
		protected abstract void SerializeMessage(Stream output, object message);

		public object Deserialize(Stream input)
		{
			try
			{
				this.log.Verbose(Diagnostics.Deserializing, input.CanSeek ? (object)input.Length : "unknown");
				return this.DeserializeMessage(input);
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception e)
			{
				this.log.Error(Diagnostics.SerializationFailed);
				throw new SerializationException(Diagnostics.SerializationFailed, e);
			}
		}
		protected abstract object DeserializeMessage(Stream input);
	}
}