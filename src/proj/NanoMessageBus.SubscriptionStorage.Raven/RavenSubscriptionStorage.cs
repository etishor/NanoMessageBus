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

            string subscriber = address.ToString();

            using(var tx= NewTransaction())
            using(var session = store.OpenSession())
            {
                foreach (string messageType in messageTypes)
                {
                    session.Advanced.DatabaseCommands.Delete(Subscription.FormatId(address.ToString(), messageType), null);
                    Subscription subscription = new Subscription(address.ToString(), messageType, expiration);
                    session.Store(subscription);
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
                foreach (string messageType in messageTypes)
                {
                    session.Advanced.DatabaseCommands.Delete(Subscription.FormatId(address.ToString(), messageType), null);
                }
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
