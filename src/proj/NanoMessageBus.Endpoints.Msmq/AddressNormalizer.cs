namespace NanoMessageBus.Endpoints
{
	internal static class AddressNormalizer
	{
		public static string ToQueuePath(this string address)
		{
			// TODO: translate [[msmq://]machine.domain/]queuename to \\machine\$.private\queuename
			return address;
		}
	}
}