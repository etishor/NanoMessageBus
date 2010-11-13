namespace NanoMessageBus
{
	public interface ISendMessages
	{
		void Send(params object[] messages);
		void Reply(params object[] messages);
	}
}