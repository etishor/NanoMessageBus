namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;

	public class LocalMessageDispatcher<TContainer> : IDispatchMessages
	{
		private static readonly IDictionary<Type, ICollection<Func<TContainer, Action<object>>>> Routes =
			new Dictionary<Type, ICollection<Func<TContainer, Action<object>>>>();

		private readonly TContainer container;

		public LocalMessageDispatcher(TContainer container)
		{
			this.container = container;
		}

		public static void Register<TMessage>(Func<TContainer, IHandleMessages<TMessage>> route)
			where TMessage : class
		{
			var key = typeof(TMessage);

			lock (Routes)
			{
				ICollection<Func<TContainer, Action<object>>> routes;
				if (!Routes.TryGetValue(key, out routes))
					Routes[key] = routes = new LinkedList<Func<TContainer, Action<object>>>();

				routes.Add(c => m => route(c).Handle(m as TMessage));
			}
		}

		public void Dispatch(object message, IMessageContext context)
		{
			ICollection<Func<TContainer, Action<object>>> registeredRoutes;
			if (!Routes.TryGetValue(message.GetType(), out registeredRoutes))
				return;

			foreach (var route in registeredRoutes)
			{
				if (!context.Continue)
					return;

				// TODO: logging
				var handler = route(this.container);
				handler(message);
			}
		}
	}
}