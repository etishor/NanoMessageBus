namespace NanoMessageBus.Transports
{
	using System;
	using System.Globalization;

	internal static class ExtensionMethods
	{
		public static TimeSpan Milliseconds(this int milliseconds)
		{
			return new TimeSpan(0, 0, 0, 0, milliseconds);
		}
		public static string FormatWith(this string format, params object[] values)
		{
			return string.Format(CultureInfo.InvariantCulture, format, values);
		}
		public static bool IsPopulated(this EnvelopeMessage message)
		{
			return message != null && message.LogicalMessages != null && message.LogicalMessages.Count > 0;
		}
	}
}