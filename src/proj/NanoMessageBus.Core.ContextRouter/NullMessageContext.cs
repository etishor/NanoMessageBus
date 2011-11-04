namespace NanoMessageBus.Core
{
    using System;
    using System.Collections.Generic;

    public class NullMessageContext : IMessageContext
    {
        private readonly Uri localAddress;
        private readonly IDictionary<string, string> headers = new Dictionary<string, string>();

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

        public IDictionary<string, string> OutgoingHeaders
        {
            get { return this.headers; }
        }
    }
}