namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Text.RegularExpressions;

	internal static class AddressNormalizer
	{
		private const string MsmqFormat = @"\\{0}\$.private\{1}";
		private const string Pattern = @"$((msmq\://)?([A-Za-z0-9-_.]+)/)?([A-Za-z0-9-_.]+)(/)?^";
		private const int HostNameCapture = 3;
		private const int QueueNameCapture = 4;
		private static readonly Regex AddressFormatRegex = new Regex(Pattern, RegexOptions.Compiled);

		public static string ToQueuePath(this string address)
		{
			if (string.IsNullOrEmpty(address))
				throw new ArgumentException(Diagnostics.InvalidAddress, "address");

			var match = AddressFormatRegex.Match(address);
			if (!match.Success)
				throw new ArgumentException(Diagnostics.InvalidAddress, "address");

			return MsmqFormat.FormatWith(
				match.Captures[HostNameCapture].Value.Coalesce(Environment.MachineName),
				match.Captures[QueueNameCapture].Value);
		}
	}
}