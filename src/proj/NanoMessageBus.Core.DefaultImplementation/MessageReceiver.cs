namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Logging;
	using Transports;

	public class MessageReceiver : IReceiveMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageReceiver));
		private readonly IDisposable childContainer;
		private readonly IHandleUnitOfWork unitOfWork;
		private readonly ITransportMessages messageTransport;
		private readonly IEnumerable<ITransformIncomingMessages> messageTransformers;
		private readonly ITrackMessageHandlers handlerTable;
		private bool disposed;

		public MessageReceiver(
			IDisposable childContainer,
			IHandleUnitOfWork unitOfWork,
			ITransportMessages messageTransport,
			IEnumerable<ITransformIncomingMessages> messageTransformers,
			ITrackMessageHandlers handlerTable)
		{
			this.childContainer = childContainer;
			this.unitOfWork = unitOfWork;
			this.messageTransport = messageTransport;
			this.messageTransformers = messageTransformers;
			this.handlerTable = handlerTable;
			this.Continue = true;
		}
		~MessageReceiver()
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

			Log.Debug(Diagnostics.DisposingMessageReceiver);
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
			this.Stop();
		}
		public virtual void Stop()
		{
			Log.Debug(Diagnostics.SkippingRemainingHandlers);
			this.Continue = false;
		}

		public virtual void Receive(PhysicalMessage message)
		{
			this.Current = this.TransformMessage(message);

			this.RouteMessagesToHandlers();

			Log.Debug(Diagnostics.CommittingUnitOfWork);
			this.unitOfWork.Complete();
		}
		private PhysicalMessage TransformMessage(PhysicalMessage message)
		{
			Log.Verbose(Diagnostics.PerformingTransformations);
			return this.messageTransformers.Aggregate(
				message,
				(current, transformer) => (PhysicalMessage)transformer.Transform(current));
		}
		private void RouteMessagesToHandlers()
		{
			Log.Verbose(Diagnostics.RoutingMessagesToHandlers);

			var routes = this.handlerTable.GetHandlers(this.Current.GetType());
			foreach (var route in routes.TakeWhile(route => this.Continue))
				route.Handle(this.Current);
		}
	}
}