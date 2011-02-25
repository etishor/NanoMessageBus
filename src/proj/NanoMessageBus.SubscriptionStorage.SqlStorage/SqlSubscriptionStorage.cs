namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Text;
	using System.Transactions;
	using Logging;
	using IsolationLevel = System.Transactions.IsolationLevel;

	public class SqlSubscriptionStorage : IStoreSubscriptions
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(SqlSubscriptionStorage));
		private readonly Func<IDbConnection> connectionFactory;

		public SqlSubscriptionStorage(Func<IDbConnection> connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public virtual void Subscribe(Uri address, IEnumerable<string> messageTypes, DateTime? expiration)
		{
			this.ExecuteCommand(address, messageTypes, command =>
			{
				foreach (var type in messageTypes)
					Log.Info(Diagnostics.Subscribe, address, type, expiration);

				command.CommandText = SqlStatements.InsertSubscription;
				command.AddParameter(SqlStatements.ExpirationParameter, expiration.ToNull());
			});
		}
		public virtual void Unsubscribe(Uri address, IEnumerable<string> messageTypes)
		{
			this.ExecuteCommand(address, messageTypes, command =>
			{
				foreach (var type in messageTypes)
					Log.Info(Diagnostics.Unsubscribe, address, type);

				command.CommandText = SqlStatements.DeleteSubscription;
			});
		}
		private void ExecuteCommand(Uri address, IEnumerable<string> messageTypes, Action<IDbCommand> callback)
		{
			if (address == null || messageTypes == null)
				return;

			using (var transaction = NewTransaction())
			using (var connection = this.connectionFactory())
			using (var command = connection.CreateCommand())
			{
				command.AddParameter(SqlStatements.SubscriberParameter, address.AbsoluteUri);
				callback(command);

				command.CommandText = PopulateCommand(command, messageTypes);
				command.ExecuteWrappedCommand();

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

		public virtual ICollection<Uri> GetSubscribers(IEnumerable<string> messageTypes)
		{
			using (SuppressTransaction())
			using (var connection = this.connectionFactory())
			using (var query = BuildQuery(connection, messageTypes))
				return query.ExecuteWrappedQuery().Select(GetSubscriber).ToArray();
		}
		private static IDisposable SuppressTransaction()
		{
			var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
			return new TransactionScope(TransactionScopeOption.Suppress, options);
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
		private static Uri GetSubscriber(IDataRecord record)
		{
			return new Uri((string)record[0]);
		}
	}
}