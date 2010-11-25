namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Logging;

	public class MessageHandlerTable<TContainer> : ITrackMessageHandlers
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(MessageHandlerTable<TContainer>));
		private static readonly IDictionary<Type, ICollection<Func<TContainer, IHandleMessages<object>>>> Routes =
			new Dictionary<Type, ICollection<Func<TContainer, IHandleMessages<object>>>>();
		private readonly TContainer childContainer;

		public MessageHandlerTable(TContainer childContainer)
		{
			this.childContainer = childContainer;
		}

		public static void RegisterHandler<TMessage>(IHandleMessages<TMessage> handler)
		{
			RegisterHandler(c => handler);
		}
		public static void RegisterHandler<TMessage>(Func<IHandleMessages<TMessage>> route)
		{
			RegisterHandler(c => route());
		}
		public static void RegisterHandler<TMessage>(Func<TContainer, IHandleMessages<TMessage>> route)
		{
			var key = typeof(TMessage);

			ICollection<Func<TContainer, IHandleMessages<object>>> routes;
			if (!Routes.TryGetValue(key, out routes)) // todo: prevent duplicate registrations
				Routes[key] = routes = new LinkedList<Func<TContainer, IHandleMessages<object>>>();

			Log.Debug(Diagnostics.RegisteringHandler, typeof(TMessage));
			routes.Add(c => new MessageHandler<TMessage>(route(c)));
		}

		public virtual IEnumerable<IHandleMessages<object>> GetHandlers(Type messageType)
		{
			ICollection<Func<TContainer, IHandleMessages<object>>> routeCallbacks;
			if (!Routes.TryGetValue(messageType, out routeCallbacks))
			{
				Log.Warn(Diagnostics.NoRegisteredHandlersFound, messageType);
				return new IHandleMessages<object>[] { };
			}

			Log.Verbose(Diagnostics.GettingHandlers, messageType);
			return routeCallbacks.Select(route => route(this.childContainer));
		}

		private class MessageHandler<TMessage> : IHandleMessages<object>
		{
			private readonly IHandleMessages<TMessage> handler;

			public MessageHandler(IHandleMessages<TMessage> handler)
			{
				this.handler = handler;
			}

			public void Handle(object message)
			{
				Log.Verbose(Diagnostics.RoutingLogicalMessageToHandler, message.GetType(), this.handler.GetType());
				this.handler.Handle((TMessage)message);
				Log.Debug(Diagnostics.HandlerCompleted, message.GetType(), this.handler.GetType());
			}
		}
	}
}