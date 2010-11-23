namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Globalization;

	internal static class ExtensionMethods
	{
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
	}
}