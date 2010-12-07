namespace NanoMessageBus.Core
{
	using System;

	/// <summary>
	/// Indicates the ability to manage a unit of work.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface should be designed to be single threaded and
	/// should not be shared between threads.
	/// </remarks>
	public interface IHandleUnitOfWork : IDisposable
	{
		/// <summary>
		/// Completes the unit of work.
		/// </summary>
		void Complete();
	}
}