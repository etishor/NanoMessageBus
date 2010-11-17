namespace NanoMessageBus.Core
{
	public interface IDispatchMessages
	{
		void Dispatch(object message, IMessageContext context);
	}
}