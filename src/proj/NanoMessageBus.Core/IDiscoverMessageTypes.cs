namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Indicates the ability to discover all associated types with a particular message.
	/// </summary>
	public interface IDiscoverMessageTypes
	{
		/// <summary>
		/// Gets all types associated with the message provided.
		/// </summary>
		/// <param name="message">The message to be inspected.</param>
		/// <returns>Returns all types (including interfaces) associated with the message provided.</returns>
		IEnumerable<Type> GetTypes(object message);

		/// <summary>
		/// Gets the names all types associated with the message provided.
		/// </summary>
		/// <param name="message">The message to be inspected.</param>
		/// <returns>Returns the names of all types (including interfaces) associated with the message provided.</returns>
		IEnumerable<string> GetTypeNames(object message);
	}
}