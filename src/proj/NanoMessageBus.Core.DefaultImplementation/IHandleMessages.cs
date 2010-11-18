namespace NanoMessageBus.Core
{
	public interface IHandleMessages
	{
		void Handle(object message);
	}
}