namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Text;
	using System.Transactions;
	using IsolationLevel = System.Transactions.IsolationLevel;

	public class SqlSubscriptionStorage : IStoreSubscriptions
	{
		private readonly Func<IDbConnection> connectionFactory;

		public SqlSubscriptionStorage(Func<IDbConnection> connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public virtual void Subscribe(string address, IEnumerable<string> messageTypes, DateTime expiration)
		{
			this.ExecuteCommand(address, messageTypes, command =>
			{
				command.CommandText = SqlStatements.InsertSubscription;
				command.AddParameter(SqlStatements.ExpirationParameter, expiration.ToNull());
			});
		}
		public virtual void Unsubscribe(string address, IEnumerable<string> messageTypes)
		{
			this.ExecuteCommand(address, messageTypes, command =>
			{
				command.CommandText = SqlStatements.DeleteSubscription;
			});
		}
		private void ExecuteCommand(string address, IEnumerable<string> messageTypes, Action<IDbCommand> callback)
		{
			if (string.IsNullOrEmpty(address) || messageTypes == null)
				return;

			using (var transaction = NewTransaction())
			using (var connection = this.connectionFactory())
			using (var command = connection.CreateCommand())
			{
				command.AddParameter(SqlStatements.SubscriberParameter, address);
				callback(command);

				command.CommandText = PopulateCommand(command, messageTypes);
				command.ExecuteNonQuery();

				transaction.Complete();
			}
		}
		private static string PopulateCommand(IDbCommand command, IEnumerable<string> messageTypes)
		{
			var builder = new StringBuilder();
			var types = messageTypes.ToArray();

			for (var i = 0; i < types.Length; i++)
			{
				command.AddParameter(SqlStatements.MessageTypeParameter + i, types[i]);
				builder.AppendFormat(command.CommandText, i);
			}

			return builder.ToString();
		}
		private static TransactionScope NewTransaction()
		{
			var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
			return new TransactionScope(TransactionScopeOption.RequiresNew, options);
		}

		public virtual ICollection<string> GetSubscribers(IEnumerable<string> messageTypes)
		{
			ICollection<string> subscribers = new LinkedList<string>();

			using (SuppressTransaction())
			using (var connection = this.connectionFactory())
			using (var query = BuildQuery(connection, messageTypes))
			using (var reader = query.ExecuteReader())
			{
				while (reader.Read())
					subscribers.Add((string)reader[0]);
			}

			return subscribers;
		}
		private static IDbCommand BuildQuery(IDbConnection connection, IEnumerable<string> messageTypes)
		{
			var command = connection.CreateCommand();
			command.CommandText = SqlStatements.MessageTypeWhereParameter;
			command.AddParameter(SqlStatements.NowParameter, DateTime.UtcNow);

			var statement = PopulateCommand(command, messageTypes);
			command.CommandText = SqlStatements.GetSubscribers.FormatWith(statement);
			return command;
		}

		private static IDisposable SuppressTransaction()
		{
			var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
			return new TransactionScope(TransactionScopeOption.Suppress, options);
		}
	}
}