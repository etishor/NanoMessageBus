namespace NanoMessageBus.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;
    using Transports;

    public class MessageRouter : IRouteMessagesToHandlers
    {
        private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageRouter));
        private readonly IDisposable disposer;
        private readonly IHandleUnitOfWork unitOfWork;
        private readonly ITransportMessages messageTransport;
        private readonly ITrackMessageHandlers handlerTable;
        private readonly IHandlePoisonMessages poisonMessageHandler;
        private bool disposed;
        private readonly IDictionary<string, string> outgoingHeaders = new Dictionary<string, string>();

        public MessageRouter(
            IDisposable disposer,
            IHandleUnitOfWork unitOfWork,
            ITransportMessages messageTransport,
            ITrackMessageHandlers handlerTable,
            IHandlePoisonMessages poisonMessageHandler)
        {
            this.disposer = disposer;
            this.unitOfWork = unitOfWork;
            this.messageTransport = messageTransport;
            this.handlerTable = handlerTable;
            this.poisonMessageHandler = poisonMessageHandler;
            this.ContinueProcessing = true;
        }
        ~MessageRouter()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed || !disposing)
                return;

            this.disposed = true;

            Log.Debug(Diagnostics.DisposingMessageRouter);
            this.ContinueProcessing = false;
            this.unitOfWork.Dispose();
            this.disposer.Dispose();
        }

        public virtual EnvelopeMessage CurrentMessage { get; private set; }
        public virtual bool ContinueProcessing { get; private set; }

        public IDictionary<string, string> OutgoingHeaders
        {
            get
            {
                return outgoingHeaders;
            }
        }

        public virtual void DeferMessage()
        {
            Log.Debug(Diagnostics.DeferringMessage);
            this.messageTransport.Send(this.CurrentMessage);
            this.DropMessage();
        }
        public virtual void DropMessage()
        {
            Log.Debug(Diagnostics.SkippingRemainingHandlers);
            this.ContinueProcessing = false;
        }

        public virtual void Route(EnvelopeMessage message)
        {
            if (message == null || message.LogicalMessages == null || message.LogicalMessages.Count == 0)
            {
                // we wore unable to get a valid message from the queue.
                // probably deserialization issue, and the message has been forwarder to the poison queue.
                // this will complete the transaction removing the message from the current queue.
                Log.Debug(Diagnostics.CommittingUnitOfWork);
                this.unitOfWork.Complete();
                return;
            }

            this.CurrentMessage = message;

            if (!this.poisonMessageHandler.IsPoison(message))
                this.TryRoute(message);

            Log.Debug(Diagnostics.CommittingUnitOfWork);
            this.unitOfWork.Complete();

            this.poisonMessageHandler.ClearFailures(message);
        }
        private void TryRoute(EnvelopeMessage message)
        {
            Log.Verbose(Diagnostics.RoutingMessagesToHandlers);

            try
            {
                var routes = this.handlerTable.GetHandlers(this.CurrentMessage);
                foreach (var route in routes.TakeWhile(route => this.ContinueProcessing))
                    route.Handle(this.CurrentMessage);
            }
            catch (Exception e)
            {
                this.poisonMessageHandler.HandleFailure(message, e);
                throw;
            }
        }
    }
}