namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;

	internal static class ExtensionMethods
	{
		public static object ToNull(this DateTime? value)
		{
			return !value.HasValue || value == DateTime.MinValue || value == DateTime.MaxValue ? DBNull.Value : (object)value;
		}

		public static string FormatWith(this string value, params object[] values)
		{
			return string.Format(CultureInfo.InvariantCulture, value, values);
		}

		public static int ExecuteWrappedCommand(this IDbCommand command)
		{
			try
			{
				return command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				throw new SubscriptionStorageException(e.Message, e);
			}
		}

		public static IEnumerable<IDataRecord> ExecuteWrappedQuery(this IDbCommand query)
		{
			IDataReader reader;
			try
			{
				reader = query.ExecuteReader();
			}
			catch (Exception e)
			{
				throw new SubscriptionStorageException(e.Message, e);
			}

			using (reader)
				while (reader.Read())
					yield return reader;
		}
	}
}