namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MessageBuilder
	{
		private readonly IDictionary<Type, TimeSpan> timeToBeReceived;
		private readonly ICollection<Type> transientMessageTypes;
		private readonly string localAddress;

		public MessageBuilder(
			IDictionary<Type, TimeSpan> timeToBeReceived,
			ICollection<Type> transientMessageTypes,
			string localAddress)
		{
			this.timeToBeReceived = timeToBeReceived;
			this.transientMessageTypes = transientMessageTypes;
			this.localAddress = localAddress;
		}

		public virtual PhysicalMessage BuildMessage(IEnumerable<object> messages)
		{
			// TODO: durable, expiration
			return new PhysicalMessage(
				Guid.NewGuid(),
				this.localAddress,
				DateTime.MaxValue,
				true,
				new Dictionary<string, string>(),
				messages.ToArray());
		}
	}
}