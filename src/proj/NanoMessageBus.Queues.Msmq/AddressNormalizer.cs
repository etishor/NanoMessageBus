namespace NanoMessageBus.Queues.Msmq
{
	using System.Messaging;

	internal static class AddressNormalizer
	{
		public static string NormalizeAddress(this MessageQueue queue)
		{
			return queue.QueueName.NormalizeAddress();
		}
		public static string NormalizeAddress(this string address)
		{
			return address; // TODO
		}
	}
}