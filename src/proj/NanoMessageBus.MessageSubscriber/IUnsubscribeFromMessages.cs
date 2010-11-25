namespace NanoMessageBus.MessageSubscriber
{
	using System;

	public interface IUnsubscribeFromMessages
	{
		void Unsubscribe(params Type[] messageTypes);
	}
}