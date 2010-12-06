namespace NanoMessageBus.Core
{
	using System;

	public class NonTransactionalPoisonMessageHandler : IHandlePoisonMessages
	{
		public virtual bool IsPoison(TransportMessage message)
		{
			return false;
		}
		public virtual void ClearFailures(TransportMessage message)
		{
			// no-op
		}
		public virtual void HandleFailure(TransportMessage message, Exception exception)
		{
			// no-op
		}
	}
}