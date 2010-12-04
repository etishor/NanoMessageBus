namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Globalization;
	using System.Text;

	internal static class ExtensionMethods
	{
		private const string Spacer = "\r\n----\r\n";
		private const string Separator = "\r\n-----------------------\r\n\r\n";

		public static TimeSpan Milliseconds(this int milliseconds)
		{
			return new TimeSpan(0, 0, 0, 0, milliseconds);
		}
		public static string FormatWith(this string format, params object[] values)
		{
			return string.Format(CultureInfo.InvariantCulture, format, values);
		}
		public static byte[] Serialize(this Exception exception)
		{
			var builder = new StringBuilder();
			exception.Serialize(builder);
			return builder.ToString().ToByteArray();
		}
		private static void Serialize(this Exception exception, StringBuilder builder)
		{
			if (exception == null)
				return;

			if (builder.Length > 0)
				builder.Append(Separator);

			builder.Append(exception.Message);
			builder.Append(Spacer);
			builder.Append(exception.StackTrace);
		}
		private static byte[] ToByteArray(this string value)
		{
			return string.IsNullOrEmpty(value) ? new byte[] { } : Encoding.UTF8.GetBytes(value);
		}
	}
}