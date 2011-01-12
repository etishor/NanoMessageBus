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
		/// Registers a particular work item, such as sending a message, to be performed a completion.
		/// </summary>
		/// <param name="callback">The callback to be invoked which performs the actual work.</param>
		void Register(Action callback);

		/// <summary>
		/// Completes the unit of work.
		/// </summary>
		void Complete();

		/// <summary>
		/// Clears the work that has been previously registered.
		/// </summary>
		void Clear();
	}
}