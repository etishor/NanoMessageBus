namespace NanoMessageBus.Core
{
	using System;

	public class NullMessageContext : IMessageContext
	{
		private readonly Uri localAddress;

		public NullMessageContext(Uri localAddress)
		{
			this.localAddress = localAddress;
		}

		public void DeferMessage()
		{
		}
		public void DropMessage()
		{
		}
		public EnvelopeMessage CurrentMessage
		{
			get
			{
				return new EnvelopeMessage(Guid.Empty, this.localAddress, TimeSpan.Zero, false, null, null);
			}
		}
		public bool ContinueProcessing
		{
			get { return false; }
		}
	}
}