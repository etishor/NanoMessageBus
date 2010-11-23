namespace NanoMessageBus.Endpoints
{
	using System;

	internal static class ExtensionMethods
	{
		public static TimeSpan Milliseconds(this int milliseconds)
		{
			return new TimeSpan(0, 0, 0, 0, milliseconds);
		}
	}
}