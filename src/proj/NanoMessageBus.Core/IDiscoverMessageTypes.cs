namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;

	public interface IDiscoverMessageTypes
	{
		IEnumerable<Type> GetTypes(object message);
		IEnumerable<string> GetTypeNames(object message);
	}
}