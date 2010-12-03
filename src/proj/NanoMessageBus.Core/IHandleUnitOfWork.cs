namespace NanoMessageBus.Core
{
	using System;

	/// <summary>
	/// Indicates the ability to manage a unit of work.
	/// </summary>
	public interface IHandleUnitOfWork : IDisposable
	{
		/// <summary>
		/// Completes the unit of work.
		/// </summary>
		void Complete();
	}
}