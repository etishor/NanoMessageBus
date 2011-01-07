namespace NanoMessageBus.Core
{
	using System;

	public class NonTransactionalPoisonMessageHandler : IHandlePoisonMessages
	{
		public virtual bool IsPoison(EnvelopeMessage message)
		{
			return false;
		}
		public virtual void ClearFailures(EnvelopeMessage message)
		{
			// no-op
		}
		public virtual void HandleFailure(EnvelopeMessage message, Exception exception)
		{
			// no-op
		}
	}
}