namespace NanoMessageBus.Serialization
{
	public interface ITransformMessages
	{
		object Transform(object message);
	}
}