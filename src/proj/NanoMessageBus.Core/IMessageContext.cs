namespace NanoMessageBus.Core
{
	public interface IMessageContext
	{
		PhysicalMessage Current { get; }
		bool Continue { get; }
		void Defer();
		void Stop();
	}
}