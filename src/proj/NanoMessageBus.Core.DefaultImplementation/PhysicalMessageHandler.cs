namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Logging;

	public class PhysicalMessageHandler : IHandleMessages<PhysicalMessage>
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(PhysicalMessageHandler));
		private readonly Func<Type, IEnumerable<ITransformIncomingMessages>> transformers;
		private readonly ITrackMessageHandlers handlerTable;
		private readonly IMessageContext context;

		public PhysicalMessageHandler(
			Func<Type, IEnumerable<ITransformIncomingMessages>> transformers,
			ITrackMessageHandlers handlerTable,
			IMessageContext context)
		{
			this.transformers = transformers;
			this.handlerTable = handlerTable;
			this.context = context;
		}

		public virtual void Handle(PhysicalMessage message)
		{
			Log.Debug(Diagnostics.LogicalMessageCount, message.LogicalMessages.Count);
			foreach (var logicalMessage in message.LogicalMessages)
				this.Handle(logicalMessage);
		}
		private void Handle(object message)
		{
			message = this.TransformMessage(message);
			if (message == null)
				return;

			this.RouteLogicalMessageToHandlers(message);
		}
		private object TransformMessage(object message)
		{
			Log.Verbose(Diagnostics.OriginalLogicalMessageType, message.GetType());

			foreach (var transformer in this.transformers(message.GetType()))
				message = transformer.Transform(message);

			Log.Debug(Diagnostics.TransformedLogicalMessageType, message == null ? null : message.GetType());

			return message;
		}
		private void RouteLogicalMessageToHandlers(object message)
		{
			Log.Debug(Diagnostics.RoutingLogicalMessageToHandlers, message.GetType());

			var handlers = this.handlerTable.GetHandlers(message.GetType());
			foreach (var handler in handlers.TakeWhile(handler => this.context.Continue))
				handler.Handle(message);

			Log.Debug(Diagnostics.LogicalMessageHandled, message.GetType());
		}
	}
}