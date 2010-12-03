namespace NanoMessageBus.MessageSubscriber
{
	using System;

	/// <summary>
	/// Indicates the ability to dispatch subscription requests.
	/// </summary>
	public interface ISubscribeToMessages
	{
		/// <summary>
		/// Dispatches subscription requests to the endpoint indicated for the message provided.
		/// </summary>
		/// <param name="endpointAddress">The endpoint to which the subscription request should be dispatched.</param>
		/// <param name="expiration">The expiration of the subscription, if accepted.</param>
		/// <param name="messageTypes">The types of messages requested in the subscription.</param>
		void Subscribe(string endpointAddress, DateTime expiration, params Type[] messageTypes);
	}
}