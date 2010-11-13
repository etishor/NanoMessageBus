namespace NanoMessageBus.Logging
{
	using System;

	public class LogFactory
	{
		public static Func<Type, ILog> BuildLogger { get; set; }
	}
}