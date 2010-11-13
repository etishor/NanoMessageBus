namespace NanoMessageBus.Core
{
	public interface ITransformMessages<T>
	{
		T Intercept(T message);
	}
}