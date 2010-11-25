namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Text;

	internal static class StatementBuilder
	{
		public static IDbCommand BuildGetSubscribersQuery(
			this IDbConnection connection, IEnumerable<Type> messageTypes)
		{
			var command = connection.CreateCommand();
			command.AddParameter(SqlStatements.NowParameter, DateTime.UtcNow);

			var i = 0;
			var whereStatementBuilder = new StringBuilder();
			foreach (var type in messageTypes ?? new Type[] { })
			{
				command.AddParameter(SqlStatements.MessageTypeParameter + i, type);
				whereStatementBuilder.AppendFormat(SqlStatements.MessageTypeWhereParameter, i++);
			}

			command.CommandText = SqlStatements.GetSubscribers.FormatWith(whereStatementBuilder);
			return command;
		}

		private static string FormatWith(this string value, params object[] values)
		{
			return string.Format(CultureInfo.InvariantCulture, value, values);
		}
	}
}