namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal static class MessageBuilder
	{
		public static PhysicalMessage BuildPhysicalMessage(
			this IEnumerable<object> messages, string localAddress)
		{
			// TODO: durable, expiration
			return new PhysicalMessage(
				Guid.NewGuid(),
				localAddress,
				DateTime.MaxValue,
				true,
				new Dictionary<string, string>(),
				messages.ToArray());
		}
	}
}