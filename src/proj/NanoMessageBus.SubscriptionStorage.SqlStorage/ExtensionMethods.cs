namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Globalization;

	internal static class ExtensionMethods
	{
		public static object ToNull(this DateTime value)
		{
			return value == DateTime.MinValue || value == DateTime.MaxValue ? DBNull.Value : (object)value;
		}

		public static string FormatWith(this string value, params object[] values)
		{
			return string.Format(CultureInfo.InvariantCulture, value, values);
		}
	}
}