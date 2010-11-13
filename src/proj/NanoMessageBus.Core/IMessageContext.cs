namespace NanoMessageBus.Core
{
	public interface IMessageContext
	{
		PhysicalMessage Current { get; }
		void Defer();
		void Skip();
	}
}