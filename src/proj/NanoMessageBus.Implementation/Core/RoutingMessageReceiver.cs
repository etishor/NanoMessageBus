namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Logging;
	using Transports;

	public class RoutingMessageReceiver : IReceiveMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(RoutingMessageReceiver));
		private const string LocalEndpoint = "";
		private readonly IDisposable container;
		private readonly IHandleUnitOfWork unitOfWork;
		private readonly ITransportMessages transport;
		private readonly IEnumerable<ITransformIncomingMessages> transformers;
		private readonly IEnumerable<IHandleMessages<PhysicalMessage>> handlers;
		private bool disposed;
		public PhysicalMessage Current { get; private set; }
		public bool Continue { get; private set; }

		public RoutingMessageReceiver(
			IDisposable container,
			IHandleUnitOfWork unitOfWork,
			ITransportMessages transport,
			IEnumerable<ITransformIncomingMessages> transformers,
			IEnumerable<IHandleMessages<PhysicalMessage>> handlers)
		{
			this.container = container;
			this.transformers = transformers;
			this.handlers = handlers;
			this.transport = transport;
			this.unitOfWork = unitOfWork;
			this.Continue = true;
		}
		~RoutingMessageReceiver()
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

			Log.Debug(CoreDiagnostics.DisposingMessageReceiver);
			this.disposed = true;
			this.Continue = false;
			this.container.Dispose();
		}

		public virtual void Defer()
		{
			Log.Debug(CoreDiagnostics.DeferringMessage);
			this.transport.Send(this.Current, LocalEndpoint);
		}
		public virtual void Stop()
		{
			Log.Debug(CoreDiagnostics.SkippingRemainingHandlers);
			this.Continue = false;
		}

		public virtual void Receive(PhysicalMessage message)
		{
			this.Current = this.TransformMessage(message);
			this.RouteMessage();

			Log.Debug(CoreDiagnostics.CommittingUnitOfWork);
			this.unitOfWork.Complete();
		}
		private PhysicalMessage TransformMessage(PhysicalMessage message)
		{
			Log.Verbose(CoreDiagnostics.PerformingTransformations);
			return this.transformers.Aggregate(
				message,
				(current, transformer) => (PhysicalMessage)transformer.Transform(current));
		}

		private void RouteMessage()
		{
			Log.Verbose(CoreDiagnostics.RoutingMessagesToHandlers);
			foreach (var handler in this.handlers)
				this.RouteMessageToHandler(handler);
		}
		private void RouteMessageToHandler(IHandleMessages<PhysicalMessage> handler)
		{
			if (!this.Continue)
				return;

			Log.Verbose(CoreDiagnostics.RoutingMessageToHandler, handler.GetType());
			handler.Handle(this.Current);
		}
	}
}