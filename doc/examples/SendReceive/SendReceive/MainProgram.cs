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

			using (var container = builder.Build())
			{
                using (var uowManager = container.Resolve<IManageCurrentUnitOfWork>())
                {
                    Console.WriteLine("Publishing...");
                    var publisher = container.Resolve<IPublishMessages>();
                    for (var i = 0; i < 100; i++)
                        publisher.Publish(new MyMessage());
                    uowManager.CurrentUnitOfWork.Complete();
                }

                using (var uowManager = container.Resolve<IManageCurrentUnitOfWork>())
                {
                    Console.WriteLine("Sending...");
                    var sender = container.Resolve<ISendMessages>();
                    for (var i = 0; i < 100; i++)
                        sender.Send(new MyMessage());
                    uowManager.CurrentUnitOfWork.Complete();
                }

                using (var uowManager = container.Resolve<IManageCurrentUnitOfWork>())
                {
                    Console.WriteLine("Listening...");
                    var transport = container.Resolve<ITransportMessages>();
                    transport.StartListening();
                    uowManager.CurrentUnitOfWork.Complete();
                }

				Console.WriteLine("Waiting...");
				Console.ReadLine();
				Console.WriteLine("Stopping...");
			}
		}
	}
}