
namespace NanoMessageBus.SubscriptionStorage.Raven.Wireup
{
    using NanoMessageBus.Wireup;
    using global::Raven.Client;
    using global::Raven.Client.Document;

    public static class RavenSubscriptionStorageWireupExtensions
    {
        public static SubscriptionStorageWireup WithRavenSubscriptionStorage(this SubscriptionStorageWireup wireup, string connectionStringName)
        {
            IDocumentStore store = new DocumentStore() { ConnectionStringName = connectionStringName };
            store.Initialize();
            return wireup.WithRavenSubscriptionStorage(store);
        }

        public static SubscriptionStorageWireup WithRavenSubscriptionStorage(this SubscriptionStorageWireup wireup, IDocumentStore store)
        {
            return wireup.WithCustomSubscriptionStorage(new RavenSubscriptionStorage(store));
        }
    }
}
