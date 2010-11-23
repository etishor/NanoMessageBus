namespace NanoMessageBus.Logging
{
	using System;
	using System.Diagnostics;

	public class TraceLogAdapter : ILog
	{
		public static void MakePrimaryLogger()
		{
			LogFactory.BuildLogger = type => new TraceLogAdapter(type);
		}

		private readonly Type typeToLog;

		public TraceLogAdapter(Type typeToLog)
		{
			this.typeToLog = typeToLog;
		}

		public void Verbose(string message, params object[] values)
		{
			this.DebugWindow(message, values);
		}
		public void Debug(string message, params object[] values)
		{
			this.DebugWindow(message, values);
		}
		public void Info(string message, params object[] values)
		{
			this.TraceWindow(message, values);
		}
		public void Warn(string message, params object[] values)
		{
			this.TraceWindow(message, values);
		}
		public void Error(string message, params object[] values)
		{
			this.TraceWindow(message, values);
		}
		public void Fatal(string message, params object[] values)
		{
			this.TraceWindow(message, values);
		}

		private void DebugWindow(string message, params object[] values)
		{
			System.Diagnostics.Debug.WriteLine(message.FormatMessage(this.typeToLog, values));
		}
		private void TraceWindow(string message, params object[] values)
		{
			Trace.WriteLine(message.FormatMessage(this.typeToLog, values));
		}
	}
}