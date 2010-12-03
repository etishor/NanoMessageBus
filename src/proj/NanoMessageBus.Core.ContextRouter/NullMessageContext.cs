namespace NanoMessageBus.Core
{
	using System;

	public class NullMessageContext : IMessageContext
	{
		private static readonly TransportMessage NullMessage =
			new TransportMessage(Guid.Empty, string.Empty, TimeSpan.Zero, false, null, null);

		public void DeferMessage()
		{
		}
		public void DropMessage()
		{
		}
		public TransportMessage CurrentMessage
		{
			get { return NullMessage; }
		}
		public bool ContinueProcessing
		{
			get { return false; }
		}
	}
}