using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using System.Transactions;
using System.Threading;

namespace NanoMessageBus.SubscriptionStorage.Raven
{
    public class RavenSubscriptionStorage : IStoreSubscriptions
    {
        private readonly IDocumentStore store;

        public RavenSubscriptionStorage(IDocumentStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }

            this.store = store;
        }

        public void Subscribe(Uri address, IEnumerable<string> messageTypes, DateTime? expiration)
        {
            if (address == null || messageTypes == null)
            {
                return;
            }

            using (var tx = NewTransaction())
            using (var session = store.OpenSession())
            {
                foreach (string messageType in messageTypes)
                {
                    Subscription subscription = session.Query<Subscription>()
                        .Where(s => s.Subscriber == address.ToString() && s.MessageType == messageType)
                        .SingleOrDefault();

                    if (subscription == null)
                    {
                        subscription = new Subscription(address.ToString(), messageType, expiration);
                        session.Store(subscription);
                    }
                    else
                    {
                        subscription.Expiration = expiration;
                    }
                }
                session.SaveChanges();
                tx.Complete();
            }
        }

        public void Unsubscribe(Uri address, IEnumerable<string> messageTypes)
        {
            if (address == null || messageTypes == null)
            {
                return;
            }

            using (var tx = NewTransaction())
            using (var session = store.OpenSession())
            {
                Remove(session, address.ToString(), messageTypes);
                session.SaveChanges();
                tx.Complete();
            }
        }

        public ICollection<Uri> GetSubscribers(IEnumerable<string> messageTypes)
        {
            using (SuppressTransaction())
            using (var session = store.OpenSession())
            {
                return messageTypes.SelectMany(mt => GetSubscribers(session, mt)).Distinct().ToList()
                    .Select(s => new Uri(s)).ToList();
            }
        }

        private void Remove(IDocumentSession session, string subscriber, IEnumerable<string> messageTypes)
        {
            foreach (string messageType in messageTypes)
            {
                Remove(session, subscriber, messageType);
            }
        }

        private void Remove(IDocumentSession session, string subscriber, string messageType)
        {
            Subscription subscription = session.Query<Subscription>()
                       .Where(s => s.Subscriber == subscriber && s.MessageType == messageType).SingleOrDefault();
            if (subscription != null)
            {
                session.Delete(subscription);
            }
        }

        private IEnumerable<string> GetSubscribers(IDocumentSession session, string messageType)
        {
            return session.Query<Subscription>().Customize(c => c.WaitForNonStaleResults())
                .Where(s => s.MessageType == messageType)
                .ToArray()
                .Select(s => s.Subscriber);
        }

        private static TransactionScope NewTransaction()
        {
            var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            return new TransactionScope(TransactionScopeOption.RequiresNew, options);
        }

        private static IDisposable SuppressTransaction()
        {
            var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            return new TransactionScope(TransactionScopeOption.Suppress, options);
        }
    }
}