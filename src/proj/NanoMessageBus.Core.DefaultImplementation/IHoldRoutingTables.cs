namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;

	public interface IHoldRoutingTables
	{
		IEnumerable<IHandleMessages> GetRoutes(Type message);
	}
}