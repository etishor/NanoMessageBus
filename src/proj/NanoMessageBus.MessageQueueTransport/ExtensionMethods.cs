namespace NanoMessageBus.MessageQueueTransport
{
	internal static class ExtensionMethods
	{
		public static bool IsPopulated(this PhysicalMessage message)
		{
			return message != null && message.LogicalMessages != null && message.LogicalMessages.Count > 0;
		}
	}
}