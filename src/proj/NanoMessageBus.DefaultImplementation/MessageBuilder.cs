namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal static class MessageBuilder
	{
		public static PhysicalMessage BuildPhysicalMessage(this IEnumerable<object> messages)
		{
			return messages.BuildPhysicalMessage(Guid.Empty);
		}
		public static PhysicalMessage BuildPhysicalMessage(this IEnumerable<object> messages, Guid correlationId)
		{
			// TODO: durable, expiration, correlationId, headers, current address (as return address?), etc.)
			return new PhysicalMessage(
				Guid.NewGuid(),
				correlationId,
				null,
				DateTime.MaxValue,
				true,
				null,
				messages.ToList());
		}
	}
}