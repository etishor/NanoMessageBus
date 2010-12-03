namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Indicates the ability to store, modify, and retreive a list of subscriptions.
	/// </summary>
	public interface IStoreSubscriptions
	{
		/// <summary>
		/// Subscribes the endpoint address specified to the message types indicated.
		/// </summary>
		/// <param name="address">The endpoint address to be subscribed.</param>
		/// <param name="messageTypes">The types of messages to add the subscription.</param>
		/// <param name="expiration">The point at which the subscription for the message types expires.</param>
		void Subscribe(string address, IEnumerable<string> messageTypes, DateTime expiration);

		/// <summary>
		/// Removes the set of message types from the endpoint address indicated.
		/// </summary>
		/// <param name="address">The endpoint address to be unsubscribed.</param>
		/// <param name="messageTypes">The types of messages to remove from the subscription.</param>
		void Unsubscribe(string address, IEnumerable<string> messageTypes);

		/// <summary>
		/// Gets a collection of subscribers for the message types provided.
		/// </summary>
		/// <param name="messageTypes">The message types for which a collection of subscribers is requested.</param>
		/// <returns>A collection of all subscribers for the message types indicated.</returns>
		ICollection<string> GetSubscribers(IEnumerable<string> messageTypes);
	}
}