namespace NanoMessageBus.Endpoints.Msmq
{
	using System.Messaging;

	internal static class AddressNormalizer
	{
		public static string ToQueueAddress(this MessageQueue queue)
		{
			return string.Empty; // TODO: translate \\machine\QueueName to msqm://machine.domain/queuename
		}
		public static string ToQueuePath(this string address)
		{
			return string.Empty; // TODO: translate msmq://machine.domain/queuename to \\machine\$.private\queuename
		}
	}
}