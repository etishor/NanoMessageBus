namespace NanoMessageBus.Core
{
	using System;

	public interface IReceiveMessages : IMessageContext, IDisposable
	{
		void Receive(PhysicalMessage message);
	}
}