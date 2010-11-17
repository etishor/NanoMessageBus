namespace NanoMessageBus.Logging
{
	using System;

	public static class LogFactory
	{
		static LogFactory()
		{
			var logger = new NullLogger();
			BuildLogger = type => logger;
		}

		public static Func<Type, ILog> BuildLogger { get; set; }
	}
}