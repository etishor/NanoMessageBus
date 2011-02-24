namespace SendReceive
{
	using System;
	using Autofac;
	using NanoMessageBus;
	using NanoMessageBus.Transports;
    using NanoMessageBus.Core;

	public static class MainProgram
	{
		public static void Main()
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule(new ConfigModule());

            builder
                .Register(c => new TransactionScopeUnitOfWork())
                .As<IHandleUnitOfWork>()
                .InstancePerLifetimeScope();

			using (var container = builder.Build())
			{
                using (var scope = container.BeginLifetimeScope())
                {
                    Console.WriteLine("Publishing...");
                    var publisher = scope.Resolve<IPublishMessages>();
                    for (var i = 0; i < 100; i++)
                        publisher.Publish(new MyMessage());
                    scope.Resolve<IHandleUnitOfWork>().Complete();
                }

                using (var scope = container.BeginLifetimeScope())
                {
                    Console.WriteLine("Sending...");
                    var sender = scope.Resolve<ISendMessages>();
                    for (var i = 0; i < 100; i++)
                        sender.Send(new MyMessage());
                    scope.Resolve<IHandleUnitOfWork>().Complete();
                }

                Console.WriteLine("Listening...");
                var transport = container.Resolve<ITransportMessages>();
                transport.StartListening();

				Console.WriteLine("Waiting...");
				Console.ReadLine();
				Console.WriteLine("Stopping...");
			}
		}
	}
}