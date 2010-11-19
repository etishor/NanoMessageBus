namespace NanoMessageBus.Core
{
	using System;

	public class NullMessageContext : IMessageContext
	{
		private static readonly PhysicalMessage NullMessage =
			new PhysicalMessage(Guid.Empty, Guid.Empty, string.Empty, DateTime.MinValue, false, null, null);

		public void Defer()
		{
		}
		public void Stop()
		{
		}
		public PhysicalMessage Current
		{
			get { return NullMessage; }
		}
		public bool Continue
		{
			get { return true; }
		}
	}
}