namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;

	public interface IStoreSubscriptions
	{
		void Subscribe(string address, IEnumerable<Type> messageTypes, DateTime expiration);
		void Unsubscribe(string address, IEnumerable<Type> messageTypes);
		IEnumerable<string> GetSubscribers(Type messageType);
	}
}