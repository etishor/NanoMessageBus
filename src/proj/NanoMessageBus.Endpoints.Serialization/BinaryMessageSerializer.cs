namespace NanoMessageBus.Endpoints.Serialization
{
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using Logging;

	public class BinaryMessageSerializer : ISerializeMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(BinaryMessageSerializer));
		private readonly IFormatter formatter = new BinaryFormatter();

		public virtual void Serialize(object message, Stream output)
		{
			Log.Verbose(Messages.Serializing, message.GetType());
			this.formatter.Serialize(output, message);
		}
		public virtual object Deserialize(Stream input)
		{
			Log.Verbose(Messages.Deserializing, input.Length);
			return this.formatter.Deserialize(input);
		}
	}
}