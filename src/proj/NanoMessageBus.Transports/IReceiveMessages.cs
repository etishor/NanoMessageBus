namespace NanoMessageBus.Transports
{
	/// <summary>
	/// Indicates the ability to receive messages.
	/// </summary>
	public interface IReceiveMessages
	{
		/// <summary>
		/// Starts the receipt of messages.
		/// </summary>
		void Start();

		/// <summary>
		/// Stops receiving new messages.
		/// </summary>
		void Stop();

		/// <summary>
		/// Forcefully aborts the receipt of new messages and kills all workers currently processing messages.
		/// </summary>
		void Abort();
	}
}