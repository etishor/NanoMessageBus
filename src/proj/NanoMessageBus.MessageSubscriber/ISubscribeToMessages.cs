namespace NanoMessageBus.MessageSubscriber
{
	using System;

	/// <summary>
	/// Indicates the ability to dispatch subscription requests.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface must be designed to be multi-thread safe.
	/// </remarks>
	public interface ISubscribeToMessages
	{
		/// <summary>
		/// Dispatches subscription requests to the endpoint indicated for the message provided.
		/// </summary>
		/// <param name="endpointAddress">The endpoint to which the subscription request should be dispatched.</param>
		/// <param name="expiration">The expiration of the subscription, if accepted.</param>
		/// <param name="messageTypes">The types of messages requested in the subscription.</param>
		void Subscribe(Uri endpointAddress, DateTime expiration, params Type[] messageTypes);
	}
}