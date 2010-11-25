namespace NanoMessageBus.MessageSubscriber
{
	using System;

	public interface ISubscribeToMessages
	{
		void Subscribe(string endpointAddress, DateTime expiration, params Type[] messageTypes);
	}
}