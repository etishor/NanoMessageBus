namespace NanoMessageBus.Wireup
{
	using Logging;

	public class LoggingWireup : WireupModule
	{
		public LoggingWireup(IWireup wireup)
			: base(wireup)
		{
		}

		public virtual LoggingWireup UseConsoleWindow()
		{
			ConsoleWindowLogger.MakePrimaryLogger();
			return this;
		}
		public virtual LoggingWireup UseOutputWindow()
		{
			OutputWindowLogger.MakePrimaryLogger();
			return this;
		}
	}
}