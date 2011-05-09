
namespace NanoMessageBus.SubscriptionStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An in-memory subscription storage. Useful for debugging purposes.
    /// </summary>
    public class InMemorySubscriptionStorage : IStoreSubscriptions
    {
        private readonly List<Subscription> subscriptions = new List<Subscription>();

        private class Subscription
        {
            public readonly Uri Address;
            public readonly string MessageType;
            public readonly DateTime? expiration;

            public Subscription(Uri address, string messageType, DateTime? expiration)
            {
                this.Address = address;
                this.MessageType = messageType;
                this.expiration = expiration;
            }
        }

        /// <summary>
        /// Adds the endpoint address specified to the message types indicated.
        /// </summary>
        /// <param name="address">The endpoint address to be subscribed.</param>
        /// <param name="messageTypes">The types of messages to add the subscription.</param>
        /// <param name="expiration">The point at which the subscription for the message types expires.</param>
        public void Subscribe(Uri address, IEnumerable<string> messageTypes, DateTime? expiration)
        {
            lock (subscriptions)
            {
                foreach (string type in messageTypes)
                {
                    subscriptions.RemoveAll(s => s.Address == address && s.MessageType == type);
                    subscriptions.Add(new Subscription(address, type, expiration));
                }
            }
        }

        /// <summary>
        /// Removes the set of message types from the endpoint address indicated.
        /// </summary>
        /// <param name="address">The endpoint address to be unsubscribed.</param>
        /// <param name="messageTypes">The types of messages to remove from the subscription.</param>
        public void Unsubscribe(Uri address, IEnumerable<string> messageTypes)
        {
            lock (subscriptions)
            {
                subscriptions.RemoveAll(s => s.Address == address && messageTypes.Any(m => m == s.MessageType));
            }
        }

        /// <summary>
        /// Gets a collection of subscribers for the message types provided.
        /// </summary>
        /// <param name="messageTypes">The message types for which a collection of subscribers is requested.</param>
        /// <returns>
        /// A collection of all subscribers for the message types indicated.
        /// </returns>
        public ICollection<Uri> GetSubscribers(IEnumerable<string> messageTypes)
        {
            lock (subscriptions)
            {
                return subscriptions
                    .Where(s => messageTypes.Any(m => s.MessageType == m))
                    .Select(s => s.Address).Distinct().ToList();
            }
        }
    }
}
