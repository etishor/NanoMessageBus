namespace NanoMessageBus.Logging
{
	using System;

	/// <summary>
	/// Provides the ability to get a new instance of the configured logger.
	/// </summary>
	public static class LogFactory
	{
		/// <summary>
		/// Initializes static members of the LogFactory class.
		/// </summary>
		static LogFactory()
		{
			var logger = new NullLogger();
			BuildLogger = type => logger;
		}

		/// <summary>
		/// Gets or sets the log builder of the configured logger.  This should be invoked to return a new logging instance.
		/// </summary>
		public static Func<Type, ILog> BuildLogger { get; set; }
	}
}