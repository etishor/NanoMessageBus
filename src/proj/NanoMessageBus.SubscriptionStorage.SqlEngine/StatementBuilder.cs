namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Text;

	internal static class StatementBuilder
	{
		private const string ParameterNamePrefix = "@p";

		public static IDbCommand BuildGetSubscribersQuery(
			this IDbConnection connection, IEnumerable<Type> messageTypes)
		{
			var command = connection.CreateCommand();

			var statementBuilder = new StringBuilder();
			var i = 0;
			foreach (var type in messageTypes ?? new Type[] { })
			{
				////command.AddParameter(ParameterNamePrefix + i, type);
				////statementBuilder.AppendFormat(
				////    CultureInfo.InvariantCulture, SqlStatements.SelectSubscribersWhere, i);
				////i++;
			}

			return command;
		}
	}
}