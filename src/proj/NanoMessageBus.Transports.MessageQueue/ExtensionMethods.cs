namespace NanoMessageBus.Transports
{
	using System;
	using System.Collections.Generic;
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
		public static bool IsPopulated(this TransportMessage message)
		{
			return message != null && message.LogicalMessages != null && message.LogicalMessages.Count > 0;
		}
		public static int Increment(this IDictionary<Guid, int> counts, Guid key)
		{
			lock (counts)
			{
				int count;
				counts.TryGetValue(key, out count);
				counts[key] = ++count;
				return count;
			}
		}
	}
}