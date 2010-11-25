namespace NanoMessageBus.MessageSubscriber
{
	using System;

	public interface ISubscribeToMessages
	{
		void Subscribe(DateTime expiration, params Type[] messageTypes);
	}
}