namespace NanoMessageBus.Core
{
	using System;

	/// <summary>
	/// Indicates the ability to route an incoming transport message to all registered message handlers.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface should be designed to be single threaded and
	/// should not be shared between threads.
	/// </remarks>
	public interface IRouteMessagesToHandlers : IMessageContext, IDisposable
	{
		/// <summary>
		/// Routes the message provided to all registered message handlers.
		/// </summary>
		/// <param name="message">The message to be routed.</param>
		void Route(TransportMessage message);
	}
}