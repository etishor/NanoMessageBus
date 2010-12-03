namespace NanoMessageBus.MessageSubscriber
{
	using System;

	/// <summary>
	/// Indicates the ability to dispatch a request to unsubscribe.
	/// </summary>
	public interface IUnsubscribeFromMessages
	{
		/// <summary>
		/// Dispatches a request to unsubscribe from the types of messages indicated.
		/// </summary>
		/// <param name="endpointAddress">The endpoint responsible for handling the unsubscribe request.</param>
		/// <param name="messageTypes">The types of messages to be unsubscribed, if successful.</param>
		void Unsubscribe(string endpointAddress, params Type[] messageTypes);
	}
}