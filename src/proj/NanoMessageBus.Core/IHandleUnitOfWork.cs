namespace NanoMessageBus.Core
{
	using System;

	public interface IHandleUnitOfWork : IDisposable
	{
		void Complete();
	}
}