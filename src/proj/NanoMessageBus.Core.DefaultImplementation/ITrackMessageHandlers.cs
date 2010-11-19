namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;

	public interface ITrackMessageHandlers
	{
		IEnumerable<IHandleMessages<object>> GetHandlers(Type message);
	}
}