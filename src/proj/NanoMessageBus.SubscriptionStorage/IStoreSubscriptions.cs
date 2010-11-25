namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;

	public interface IStoreSubscriptions
	{
		void Subscribe(string address, IEnumerable<string> messageTypes, DateTime expiration);
		void Unsubscribe(string address, IEnumerable<string> messageTypes);
		ICollection<string> GetSubscribers(IEnumerable<string> messageTypes);
	}
}