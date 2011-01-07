namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;

	public class MessageBuilder
	{
		private readonly IDictionary<Type, TimeSpan> timeToLive;
		private readonly ICollection<Type> transientMessages;
		private readonly Uri localAddress;

		public MessageBuilder(Uri localAddress)
			: this(null, null, localAddress)
		{
		}
		public MessageBuilder(
			IDictionary<Type, TimeSpan> timeToLive, ICollection<Type> transientMessages, Uri localAddress)
		{
			this.timeToLive = timeToLive ?? new Dictionary<Type, TimeSpan>();
			this.transientMessages = transientMessages ?? new HashSet<Type>();
			this.localAddress = localAddress;
		}

		public virtual EnvelopeMessage BuildMessage(params object[] messages)
		{
			if (messages == null || 0 == messages.Length)
				return null;

			var primaryMessageType = messages[0].GetType();
			return new EnvelopeMessage(
				Guid.NewGuid(),
				this.localAddress,
				this.GetTimeToLive(primaryMessageType),
				this.IsPersistent(primaryMessageType),
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
		private bool IsPersistent(Type messageType)
		{
			return !this.transientMessages.Contains(messageType);
		}

		public virtual void RegisterMaximumMessageLifetime(Type messageType, TimeSpan ttl)
		{
			this.timeToLive[messageType] = ttl;
		}
		public virtual void RegisterTransientMessage(Type messageType)
		{
			this.transientMessages.Add(messageType);
		}
	}
}