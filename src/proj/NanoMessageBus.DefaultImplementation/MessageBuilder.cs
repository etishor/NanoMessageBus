namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;

	public class MessageBuilder
	{
		private readonly IDictionary<Type, TimeSpan> timeToLive;
		private readonly ICollection<Type> transientMessages;
		private readonly string localAddress;

		public MessageBuilder(
			IDictionary<Type, TimeSpan> timeToLive,
			ICollection<Type> transientMessages,
			string localAddress)
		{
			this.timeToLive = timeToLive;
			this.transientMessages = transientMessages;
			this.localAddress = localAddress;
		}

		public virtual PhysicalMessage BuildMessage(params object[] messages)
		{
			if (messages == null || 0 == messages.Length)
				return null;

			var primaryMessageType = messages[0].GetType();
			return new PhysicalMessage(
				Guid.NewGuid(),
				this.localAddress,
				this.GetTimeToLive(primaryMessageType),
				this.IsMessageDurable(primaryMessageType),
				new Dictionary<string, string>(),
				messages);
		}
		private TimeSpan GetTimeToLive(Type messageType)
		{
			TimeSpan ttl;
			if (this.timeToLive.TryGetValue(messageType, out ttl))
				return ttl;

			return TimeSpan.MaxValue;
		}
		private bool IsMessageDurable(Type messageType)
		{
			return !this.transientMessages.Contains(messageType);
		}
	}
}