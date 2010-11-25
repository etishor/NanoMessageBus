namespace NanoMessageBus.MessageSubscriber
{
	using System;

	public interface IUnsubscribeFromMessages
	{
		void Unsubscribe(string endpointAddress, params Type[] messageTypes);
	}
}