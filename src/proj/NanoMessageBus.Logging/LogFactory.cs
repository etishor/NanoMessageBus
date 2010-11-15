namespace NanoMessageBus.Logging
{
	using System;

	public static class LogFactory
	{
		public static Func<Type, ILog> BuildLogger { get; set; }
	}
}