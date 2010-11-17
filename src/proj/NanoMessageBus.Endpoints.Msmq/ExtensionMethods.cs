namespace NanoMessageBus.Endpoints
{
	using System;

	internal static class ExtensionMethods
	{
		public static TimeSpan Seconds(this int seconds)
		{
			return new TimeSpan(0, 0, 0, seconds);
		}
	}
}