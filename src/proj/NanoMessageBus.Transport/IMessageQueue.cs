namespace NanoMessageBus.Transport
{
	using System;
	using Core;

	public interface IMessageQueue : IDisposable
	{
		void Enqueue(PhysicalMessage message);

		event EventHandler MessageAvailable;
		PhysicalMessage Dequeue();
	}
}