namespace NanoMessageBus.Wireup
{
	using System;
	using Autofac;
	using Core;

	public class MessageHandlerWireup : WireupModule
	{
		public MessageHandlerWireup(IWireup wireup)
			: base(wireup)
		{
		}

		public virtual MessageHandlerWireup AddHandler<TMessage>(IHandleMessages<TMessage> handler)
		{
			return this.AddHandler(c => handler);
		}
		public virtual MessageHandlerWireup AddHandler<TMessage>(Func<IHandleMessages<TMessage>> route)
		{
			return this.AddHandler(c => route());
		}
		public virtual MessageHandlerWireup AddHandler<TMessage>(
			Func<IComponentContext, IHandleMessages<TMessage>> route)
		{
			MessageHandlerTable<IComponentContext>.RegisterHandler(route);
			return this;
		}
	}
}