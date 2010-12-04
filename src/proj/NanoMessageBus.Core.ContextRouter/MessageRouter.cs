namespace NanoMessageBus.Core
{
	using System;
	using System.Linq;
	using Logging;
	using Transports;

	public class MessageRouter : IRouteMessagesToHandlers
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageRouter));
		private readonly IDisposable childContainer;
		private readonly IHandleUnitOfWork unitOfWork;
		private readonly ITransportMessages messageTransport;
		private readonly ITrackMessageHandlers handlerTable;
		private readonly IHandlePoisonMessages poisonMessageHandler;
		private bool disposed;

		public MessageRouter(
			IDisposable childContainer,
			IHandleUnitOfWork unitOfWork,
			ITransportMessages messageTransport,
			ITrackMessageHandlers handlerTable,
			IHandlePoisonMessages poisonMessageHandler)
		{
			this.childContainer = childContainer;
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
			this.childContainer.Dispose();
		}

		public virtual TransportMessage CurrentMessage { get; private set; }
		public virtual bool ContinueProcessing { get; private set; }

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

		public virtual void Route(TransportMessage message)
		{
			this.CurrentMessage = message;

			if (!this.poisonMessageHandler.IsPoison(message))
				this.TryRoute(message);

			Log.Debug(Diagnostics.CommittingUnitOfWork);
			this.unitOfWork.Complete();

			this.poisonMessageHandler.HandleSuccess(message);
		}
		private void TryRoute(TransportMessage message)
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