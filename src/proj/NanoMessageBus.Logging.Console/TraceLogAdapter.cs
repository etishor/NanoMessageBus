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

		private static readonly object Sync = new object();
		private readonly Type typeToLog;

		public TraceLogAdapter(Type typeToLog)
		{
			this.typeToLog = typeToLog;
		}

		public void Verbose(string message, params object[] values)
		{
			this.DebugWindow("Verbose / ", message, values);
		}
		public void Debug(string message, params object[] values)
		{
			this.DebugWindow("Debug / ", message, values);
		}
		public void Info(string message, params object[] values)
		{
			this.TraceWindow("Info / ", message, values);
		}
		public void Warn(string message, params object[] values)
		{
			this.TraceWindow("Warn / ", message, values);
		}
		public void Error(string message, params object[] values)
		{
			this.TraceWindow("Error / ", message, values);
		}
		public void Fatal(string message, params object[] values)
		{
			this.TraceWindow("Fatal / ", message, values);
		}

		private void DebugWindow(string category, string message, params object[] values)
		{
			lock (Sync)
				System.Diagnostics.Debug.WriteLine(category, message.FormatMessage(this.typeToLog, values));
		}
		private void TraceWindow(string category, string message, params object[] values)
		{
			lock (Sync)
				Trace.Write(category, message.FormatMessage(this.typeToLog, values));
		}
	}
}