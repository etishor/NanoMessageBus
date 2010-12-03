namespace NanoMessageBus.Core
{
	using System;

	/// <summary>
	/// Indicates the ability to route an incoming transport message to all registered message handlers.
	/// </summary>
	public interface IRouteMessagesToHandlers : IMessageContext, IDisposable
	{
		/// <summary>
		/// Routes the message provided to all registered message handlers.
		/// </summary>
		/// <param name="message">The message to be routed.</param>
		void Route(TransportMessage message);
	}
}