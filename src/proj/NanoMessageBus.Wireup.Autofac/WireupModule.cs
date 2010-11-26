namespace NanoMessageBus.Wireup
{
	using System;
	using System.Collections.Generic;
	using Autofac;

	public class WireupModule : Module, IWireup
	{
		private readonly IDictionary<Type, IWireup> modules = new Dictionary<Type, IWireup>();
		private readonly IWireup parent;

		public WireupModule()
		{
			this.modules.Add(new LoggingWireup(this));
			this.modules.Add(new TransportWireup(this));
			this.modules.Add(new EndpointWireup(this));
			this.modules.Add(new SerializationWireup(this));
			this.modules.Add(new MessageRouterWireup(this));
			this.modules.Add(new MessageHandlerWireup(this));
			this.modules.Add(new MessageBusWireup(this));
			this.modules.Add(new SubscriptionStorageWireup(this));
			this.modules.Add(new MessageSubscriberWireup(this));
		}

		protected WireupModule(IWireup parent)
		{
			this.parent = parent;
		}

		public virtual TWireup Configure<TWireup>() where TWireup : class, IWireup
		{
			if (this.parent != null)
				return this.parent.Configure<TWireup>();

			return this.modules[typeof(TWireup)] as TWireup;
		}
		public virtual TWireup Configure<TWireup>(TWireup module) where TWireup : class, IWireup
		{
			if (this.parent != null)
				return this.parent.Configure(module);

			return this.modules.Add(module);
		}
		public virtual void Register(ContainerBuilder builder)
		{
			if (this.parent == null)
				foreach (var module in this.modules.Values)
					builder.RegisterModule(module);
			else
				this.parent.Register(builder);
		}
	}
}