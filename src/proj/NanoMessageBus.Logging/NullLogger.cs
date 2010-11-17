namespace NanoMessageBus.Logging
{
	internal class NullLogger : ILog
	{
		public void Verbose(string message, params object[] values)
		{
		}
		public void Debug(string message, params object[] values)
		{
		}
		public void Info(string message, params object[] values)
		{
		}
		public void Warn(string message, params object[] values)
		{
		}
		public void Error(string message, params object[] values)
		{
		}
		public void Fatal(string message, params object[] values)
		{
		}
	}
}