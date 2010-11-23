namespace NanoMessageBus.Transports
{
	public interface IReceiveMessages
	{
		void Start();
		void Stop();
		void Abort();
	}
}