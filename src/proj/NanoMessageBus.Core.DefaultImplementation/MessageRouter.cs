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
		private bool disposed;

		public MessageRouter(
			IDisposable childContainer,
			IHandleUnitOfWork unitOfWork,
			ITransportMessages messageTransport,
			ITrackMessageHandlers handlerTable)
		{
			this.childContainer = childContainer;
			this.unitOfWork = unitOfWork;
			this.messageTransport = messageTransport;
			this.handlerTable = handlerTable;
			this.Continue = true;
		}
		~MessageRouter()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.Dispose(true);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed || !disposing)
				return;

			Log.Debug(Diagnostics.DisposingMessageRouter);
			this.disposed = true;
			this.Continue = false;
			this.childContainer.Dispose();
		}

		public virtual PhysicalMessage Current { get; private set; }
		public virtual bool Continue { get; private set; }

		public virtual void Defer()
		{
			Log.Debug(Diagnostics.DeferringMessage);
			this.messageTransport.Send(this.Current);
			this.Drop();
		}
		public virtual void Drop()
		{
			Log.Debug(Diagnostics.SkippingRemainingHandlers);
			this.Continue = false;
		}

		public virtual void Route(PhysicalMessage message)
		{
			this.Current = message;

			Log.Verbose(Diagnostics.RoutingMessagesToHandlers);

			var routes = this.handlerTable.GetHandlers(this.Current.GetType());
			foreach (var route in routes.TakeWhile(route => this.Continue))
				route.Handle(this.Current);

			Log.Debug(Diagnostics.CommittingUnitOfWork);
			this.unitOfWork.Complete();
		}
	}
}