namespace NanoMessageBus.Endpoints
{
	using System;
	using System.Messaging;
	using System.Text.RegularExpressions;

	internal static class AddressNormalizer
	{
		private const string PrivateQueue = @"PRIVATE$\";
		private const string MsmqFormat = @"{0}\PRIVATE$\{1}";
		private const string CanonicalFormat = @"msmq://{0}/{1}";
		private const string Pattern = @"^((msmq\://)?([A-Za-z0-9-_.]+)/)?([A-Za-z0-9-_.]+)(/)?$";
		private const int HostNameCapture = 3;
		private const int QueueNameCapture = 4;
		private static readonly Regex AddressFormatRegex = new Regex(Pattern, RegexOptions.Compiled);

		public static string ToQueuePath(this string address)
		{
			if (string.IsNullOrEmpty(address))
				throw new ArgumentException(Diagnostics.InvalidAddress.FormatWith(address), "address");

			var match = AddressFormatRegex.Match(address);
			if (!match.Success)
				throw new ArgumentException(Diagnostics.InvalidAddress.FormatWith(address), "address");

			var machineName = match.Groups[HostNameCapture].Value.GetMachineName();
			var queueName = match.Groups[QueueNameCapture].Value;
			return MsmqFormat.FormatWith(machineName, queueName);
		}

		public static string CanonicalAddress(this MessageQueue queue)
		{
			return CanonicalFormat.FormatWith(
				queue.MachineName, queue.QueueName.Replace(PrivateQueue, string.Empty));
		}
	}
}