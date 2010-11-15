namespace NanoMessageBus.Logging.Log4Net
{
	using System;

	public class Log4NetAdapter : ILog
	{
		public static void MakePrimaryLogger()
		{
			LogFactory.BuildLogger = type => new Log4NetAdapter(type);
		}

		private readonly log4net.ILog log;

		public Log4NetAdapter(Type typeToLog)
		{
			this.log = log4net.LogManager.GetLogger(typeToLog);
		}

		public void Verbose(string message, params object[] values)
		{
			// TODO
			if (this.log.IsDebugEnabled)
				this.log.DebugFormat(message, values);
		}
		public void Debug(string message, params object[] values)
		{
			if (this.log.IsDebugEnabled)
				this.log.DebugFormat(message, values);
		}
		public void Info(string message, params object[] values)
		{
			if (this.log.IsInfoEnabled)
				this.log.InfoFormat(message, values);
		}
		public void Warn(string message, params object[] values)
		{
			if (this.log.IsWarnEnabled)
				this.log.WarnFormat(message, values);
		}
		public void Error(string message, params object[] values)
		{
			if (this.log.IsErrorEnabled)
				this.log.ErrorFormat(message, values);
		}
		public void Fatal(string message, params object[] values)
		{
			if (this.log.IsFatalEnabled)
				this.log.FatalFormat(message, values);
		}
	}
}