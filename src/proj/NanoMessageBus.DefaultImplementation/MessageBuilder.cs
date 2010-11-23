namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal static class MessageBuilder
	{
		public static PhysicalMessage BuildPhysicalMessage(this IEnumerable<object> messages)
		{
			// TODO: durable, expiration, headers, current address (as return address?), etc.)
			return new PhysicalMessage(
				Guid.NewGuid(),
				0,
				0,
				null,
				DateTime.MaxValue,
				true,
				null,
				messages.ToList());
		}
	}
}