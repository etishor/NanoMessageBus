namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Transactions;
	using IsolationLevel = System.Transactions.IsolationLevel;

	public class SqlSubscriptionStorage : IStoreSubscriptions
	{
		private readonly IDbConnection connection;

		public SqlSubscriptionStorage(IDbConnection connection)
		{
			this.connection = connection;
		}

		public virtual void Subscribe(string address, IEnumerable<Type> messageTypes, DateTime expiration)
		{
			if (string.IsNullOrEmpty(address))
				return;

			using (var transaction = NewTransaction())
			using (var command = this.connection.CreateCommand())
			{
				command.CommandText = string.Empty;
				command.Prepare();
				command.AddParameter(string.Empty, string.Empty);
				command.AddParameter("@now", DateTime.UtcNow);

				foreach (var messageType in messageTypes ?? new Type[] { })
				{
					((IDataParameter)command.Parameters[0]).Value = messageType;
					command.ExecuteNonQuery();	
				}

				transaction.Complete();
			}
		}
		public virtual void Unsubscribe(string address, IEnumerable<Type> messageTypes)
		{
			if (string.IsNullOrEmpty(address))
				return;

			using (var transaction = NewTransaction())
			{
				transaction.Complete();
			}
		}
		public virtual ICollection<string> GetSubscribers(IEnumerable<Type> messageTypes)
		{
			ICollection<string> subscribers = new LinkedList<string>();

			using (SuppressTransaction())
			using (var query = this.connection.BuildGetSubscribersQuery(messageTypes))
			using (var reader = query.ExecuteReader())
			{
				while (reader.Read())
					subscribers.Add((string)reader[0]);
			}

			return subscribers;
		}

		private static TransactionScope NewTransaction()
		{
			return new TransactionScope(
				TransactionScopeOption.RequiresNew,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
		}
		private static IDisposable SuppressTransaction()
		{
			return new TransactionScope(TransactionScopeOption.Suppress);
		}
	}
}