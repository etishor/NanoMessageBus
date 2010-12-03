namespace NanoMessageBus.Transports
{
	using System.Threading;

	public class BackgroundThread : IThread
	{
		private readonly Thread thread;

		public BackgroundThread(ThreadStart startAction)
		{
			this.thread = new Thread(startAction)
			{
				IsBackground = true
			};

			this.thread.Name = Diagnostics.WorkerThreadName.FormatWith(this.thread.ManagedThreadId);
		}

		public virtual bool IsAlive
		{
			get { return this.thread.IsAlive; }
		}
		public virtual string Name
		{
			get { return this.thread.Name; }
		}
		public virtual void Start()
		{
			this.thread.Start();
		}
		public virtual void Abort()
		{
			this.thread.Abort();
		}
	}
}