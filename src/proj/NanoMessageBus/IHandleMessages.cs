namespace NanoMessageBus
{
	public interface IHandleMessages<T>
	{
		void Handle(T message);
	}
}