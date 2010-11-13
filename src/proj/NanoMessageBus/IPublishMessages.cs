namespace NanoMessageBus
{
	public interface IPublishMessages
	{
		void Publish(params object[] messages);
	}
}