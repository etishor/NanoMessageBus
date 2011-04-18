namespace NanoMessageBus.Wireup
{
	using System.Configuration;
	using System.Data;
	using System.Data.Common;
	using System.Data.SqlClient;
	using Autofac;
	using SubscriptionStorage;

	public class SubscriptionStorageWireup : WireupModule
	{
		private const string DefaultProviderName = "System.Data.SqlClient";
		private string connectionString;
		private string providerName;

        private IStoreSubscriptions customStorage;

		public SubscriptionStorageWireup(IWireup parent)
			: base(parent)
		{
		}

		public virtual SubscriptionStorageWireup ConnectTo(string connectionName)
		{
			var settings = ConfigurationManager.ConnectionStrings[connectionName];
			return this.ConnectTo(settings.ConnectionString, settings.ProviderName);
		}
		public virtual SubscriptionStorageWireup ConnectTo(string connection, string provider)
		{
			this.connectionString = connection;
			this.providerName = provider ?? DefaultProviderName;
			return this;
		}

        public virtual SubscriptionStorageWireup WithCustomSubscriptionStorage(IStoreSubscriptions storage)
        {
            this.customStorage = storage;
            return this;
        }

		protected override void Load(ContainerBuilder builder)
		{
			builder
				.Register(this.BuildSubscriptionStorage)
				.As<IStoreSubscriptions>()
				.SingleInstance()
				.ExternallyOwned();
		}
		protected virtual IStoreSubscriptions BuildSubscriptionStorage(IComponentContext c)
		{
            if (this.customStorage != null)
            {
                return this.customStorage;
            }

			return new SqlSubscriptionStorage(this.OpenConnection);

		}
		private IDbConnection OpenConnection()
		{
			var factory = DbProviderFactories.GetFactory(this.providerName);
			var connection = factory.CreateConnection() ?? new SqlConnection();
			connection.ConnectionString = this.connectionString;
			connection.Open();
			return connection;
		}
	}
}