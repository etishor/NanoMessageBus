namespace NanoMessageBus.Logging
{
	using System;

	public class NLogAdapter : ILog
	{
		public static void MakePrimaryLogger()
		{
			LogFactory.BuildLogger = type => new NLogAdapter(type);
		}

		private static readonly NLog.LogFactory Factory = new NLog.LogFactory();
		private readonly global::NLog.Logger log;

		public NLogAdapter(Type typeToLog)
		{
			this.log = Factory.GetLogger(typeToLog.FullName);
		}

		public virtual void Verbose(string message, params object[] values)
		{
			if (this.log.IsTraceEnabled)
				this.log.Trace(message, values);
		}
		public virtual void Debug(string message, params object[] values)
		{
			if (this.log.IsDebugEnabled)
				this.log.Debug(message, values);
		}
		public virtual void Info(string message, params object[] values)
		{
			if (this.log.IsInfoEnabled)
				this.log.Info(message, values);
		}
		public virtual void Warn(string message, params object[] values)
		{
			if (this.log.IsWarnEnabled)
				this.log.Warn(message, values);
		}
		public virtual void Error(string message, params object[] values)
		{
			if (this.log.IsErrorEnabled)
				this.log.Error(message, values);
		}
		public virtual void Fatal(string message, params object[] values)
		{
			if (this.log.IsFatalEnabled)
				this.log.Fatal(message, values);
		}
	}
}