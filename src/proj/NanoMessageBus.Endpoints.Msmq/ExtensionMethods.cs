namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Globalization;

	internal static class ExtensionMethods
	{
		private const string LocalHost = ".";

		public static TimeSpan Milliseconds(this int milliseconds)
		{
			return new TimeSpan(0, 0, 0, 0, milliseconds);
		}
		public static string Coalesce(this string value, string alternate)
		{
			return string.IsNullOrEmpty(value) ? alternate : value;
		}
		public static string FormatWith(this string format, params object[] values)
		{
			return string.Format(CultureInfo.InvariantCulture, format, values);
		}
		public static string GetMachineName(this string value)
		{
			value = (value ?? string.Empty).Trim();
			value = value.Length == 0 || value == LocalHost ? Environment.MachineName : value;
			return value.ToLowerInvariant();
		}
	}
}