namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core;

	internal static class MessageBuilder
	{
		public static PhysicalMessage BuildPhysicalMessage(
			this IEnumerable<object> messages, IMessageContext context)
		{
			// TODO: durable, expiration, correlationId, headers, current address (as return address?), etc.)
			return new PhysicalMessage(
				Guid.NewGuid(),
				context.Current.CorrelationId,
				null,
				DateTime.MaxValue,
				true,
				null,
				messages.Where(x => x != null).ToList());
		}
	}
}