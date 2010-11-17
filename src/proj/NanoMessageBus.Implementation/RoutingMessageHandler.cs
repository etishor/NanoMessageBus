namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using Core;
	using Logging;

	public class RoutingMessageHandler : IHandleMessages<PhysicalMessage>
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(RoutingMessageHandler));
		private readonly Func<Type, IEnumerable<ITransformIncomingMessages>> transformers;
		private readonly Func<Type, IEnumerable<IHandleMessages<object>>> handlers;
		private readonly IMessageContext context;

		public RoutingMessageHandler(
			Func<Type, IEnumerable<ITransformIncomingMessages>> transformers,
			Func<Type, IEnumerable<IHandleMessages<object>>> handlers,
			IMessageContext context)
		{
			// TODO: logging
			// TODO: generics vs delegates

			this.transformers = transformers;
			this.context = context;
			this.handlers = handlers;
		}

		public virtual void Handle(PhysicalMessage message)
		{
			foreach (var logicalMessage in message.LogicalMessages)
				this.Handle(logicalMessage);
		}
		private void Handle(object message)
		{
			if (!this.context.Continue)
				return;

			foreach (var transformer in this.transformers(message.GetType()))
				message = transformer.Transform(message);

			if (message == null)
				return;

			foreach (var handler in this.handlers(message.GetType()))
				this.RouteToHandler(handler, message);
		}
		private void RouteToHandler(IHandleMessages<object> handler, object message)
		{
			if (this.context.Continue)
				handler.Handle(message);
		}
	}
}