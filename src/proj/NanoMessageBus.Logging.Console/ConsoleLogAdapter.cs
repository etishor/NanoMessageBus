namespace NanoMessageBus.Logging
{
	using System;

	public class ConsoleLogAdapter : ILog
	{
		public static void MakePrimaryLogger()
		{
			LogFactory.BuildLogger = type => new ConsoleLogAdapter(type);
		}

		private static readonly object Sync = new object();
		private readonly Type typeToLog;
		private readonly ConsoleColor originalColor = Console.ForegroundColor;

		public ConsoleLogAdapter(Type typeToLog)
		{
			this.typeToLog = typeToLog;
		}

		public void Verbose(string message, params object[] values)
		{
			this.Log(ConsoleColor.Green, message, values);
		}
		public void Debug(string message, params object[] values)
		{
			this.Log(ConsoleColor.Green, message, values);
		}
		public void Info(string message, params object[] values)
		{
			this.Log(ConsoleColor.White, message, values);
		}
		public void Warn(string message, params object[] values)
		{
			this.Log(ConsoleColor.Yellow, message, values);
		}
		public void Error(string message, params object[] values)
		{
			this.Log(ConsoleColor.Red, message, values);
		}
		public void Fatal(string message, params object[] values)
		{
			this.Log(ConsoleColor.Red, message, values);
		}

		private void Log(ConsoleColor color, string message, params object[] values)
		{
			lock (Sync)
			{
				Console.ForegroundColor = color;
				Console.Write(message.FormatMessage(this.typeToLog, values));
				Console.ForegroundColor = this.originalColor;
			}
		}
	}
}