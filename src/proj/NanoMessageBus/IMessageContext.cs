namespace NanoMessageBus
{
	public interface IMessageContext
	{
		PhysicalMessage Current { get; }
		bool Continue { get; }
		void Defer();
		void Drop();
	}
}