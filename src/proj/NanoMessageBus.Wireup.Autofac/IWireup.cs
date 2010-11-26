namespace NanoMessageBus.Wireup
{
	using Autofac;
	using Autofac.Core;

	public interface IWireup : IModule
	{
		TWireup Configure<TWireup>() where TWireup : class, IWireup;
		TWireup Configure<TWireup>(TWireup module) where TWireup : class, IWireup;
		void Register(ContainerBuilder builder);
	}
}