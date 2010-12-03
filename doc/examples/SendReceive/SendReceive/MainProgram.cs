namespace SendReceive
{
	using System;
	using Autofac;
	using NanoMessageBus;
	using NanoMessageBus.Transports;

	public static class MainProgram
	{
		public static void Main()
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule(new ConfigModule());

			using (var container = builder.Build())
			{
				Console.WriteLine("Publishing...");
				container.Resolve<IPublishMessages>().Publish(new MyMessage());

				Console.WriteLine("Sending...");
				var sender = container.Resolve<ISendMessages>();
				for (var i = 0; i < 100; i++)
					sender.Send(new MyMessage());

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