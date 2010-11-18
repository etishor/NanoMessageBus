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
		private readonly IDispatchMessages messageRouter;
		private bool disposed;
		public PhysicalMessage Current { get; private set; }
		public bool Continue { get; private set; }

		public RoutingMessageReceiver(
			IDisposable container,
			IHandleUnitOfWork unitOfWork,
			ITransportMessages transport,
			IEnumerable<ITransformIncomingMessages> transformers,
			IDispatchMessages messageRouter)
		{
			this.container = container;
			this.unitOfWork = unitOfWork;
			this.transport = transport;
			this.transformers = transformers;
			this.messageRouter = messageRouter;
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

			Log.Debug(Diagnostics.DisposingMessageReceiver);
			this.disposed = true;
			this.Continue = false;
			this.container.Dispose();
		}

		public virtual void Defer()
		{
			Log.Debug(Diagnostics.DeferringMessage);
			this.transport.Send(this.Current, LocalEndpoint);
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

			Log.Verbose(Diagnostics.RoutingMessagesToHandlers);
			this.messageRouter.Dispatch(this.Current, this);

			Log.Debug(Diagnostics.CommittingUnitOfWork);
			this.unitOfWork.Complete();
		}
		private PhysicalMessage TransformMessage(PhysicalMessage message)
		{
			Log.Verbose(Diagnostics.PerformingTransformations);
			return this.transformers.Aggregate(
				message,
				(current, transformer) => (PhysicalMessage)transformer.Transform(current));
		}
	}
}