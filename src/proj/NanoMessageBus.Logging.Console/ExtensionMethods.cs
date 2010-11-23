namespace NanoMessageBus.Logging
{
	using System;
	using System.Globalization;
	using System.Threading;

	internal static class ExtensionMethods
	{
		private const string MessageFormat = "{0} - {1} - {2} - {3}\r\n";

		public static string FormatMessage(this string message, Type typeToLog, params object[] values)
		{
			return string.Format(
				CultureInfo.InvariantCulture,
				MessageFormat,
				DateTime.UtcNow,
				Thread.CurrentThread.Name,
				typeToLog.FullName,
				string.Format(CultureInfo.InvariantCulture, message, values));
		}
	}
}