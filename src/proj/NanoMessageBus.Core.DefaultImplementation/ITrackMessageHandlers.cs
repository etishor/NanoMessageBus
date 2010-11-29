namespace NanoMessageBus.Core
{
	using System.Collections.Generic;

	public interface ITrackMessageHandlers
	{
		IEnumerable<IHandleMessages<object>> GetHandlers(object message);
	}
}