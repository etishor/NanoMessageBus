namespace SendReceive
{
	using System;
	using NanoMessageBus;

	public class MyMessageHandler : IHandleMessages<MyMessage>
	{
		public void Handle(MyMessage message)
		{
			Console.WriteLine(message.MessageId);
		}
	}
}