namespace NanoMessageBus.Transport
{
	using System;

	public interface IHandleUnitOfWork : IDisposable
	{
		void Complete();
	}
}