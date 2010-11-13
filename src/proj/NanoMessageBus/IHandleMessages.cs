namespace NanoMessageBus
{
	public interface IHandleMessages<in T>
	{
		void Handle(T message);
	}
}