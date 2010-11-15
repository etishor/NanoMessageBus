namespace NanoMessageBus.Endpoints.Msmq
{
	internal static class AddressNormalizer
	{
		public static string ToQueuePath(this string address)
		{
			return string.Empty; // TODO: translate [[msmq://]machine.domain/]queuename to \\machine\$.private\queuename
		}
	}
}