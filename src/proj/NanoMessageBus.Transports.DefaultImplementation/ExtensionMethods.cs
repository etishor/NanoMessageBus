namespace NanoMessageBus.Transports
{
	using System.Globalization;

	internal static class ExtensionMethods
	{
		public static string FormatWith(this string format, params object[] values)
		{
			return string.Format(CultureInfo.InvariantCulture, format, values);
		}
		public static bool IsPopulated(this PhysicalMessage message)
		{
			return message != null && message.LogicalMessages != null && message.LogicalMessages.Count > 0;
		}
	}
}