namespace NanoMessageBus
{
	public interface IMessageContext
	{
		PhysicalMessage CurrentMessage { get; }
		bool ContinueProcessing { get; }
		void DeferMessage();
		void DropMessage();
	}
}