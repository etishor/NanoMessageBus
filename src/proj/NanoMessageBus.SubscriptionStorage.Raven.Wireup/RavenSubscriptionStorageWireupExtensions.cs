
namespace NanoMessageBus
{
    using global::Raven.Client;
    using global::Raven.Client.Document;
    using NanoMessageBus.SubscriptionStorage.Raven;
    using NanoMessageBus.Wireup;
    using Newtonsoft.Json;

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

        public static SerializationWireup WithRavenJsonSerializer(this SerializationWireup wireup)
        {
            return wireup.CustomSerializer(new Serialization.RavenJsonSerializer());
        }

        public static SerializationWireup WithRavenJsonSerializer(this SerializationWireup wireup, JsonSerializer customSerializer)
        {
            return wireup.CustomSerializer(new Serialization.RavenJsonSerializer(customSerializer));
        }
    }
}
