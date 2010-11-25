namespace NanoMessageBus.SubscriptionStorage
{
	using System;

	internal static class ExtensionMethods
	{
		public static object ToNull(this DateTime value)
		{
			return value == DateTime.MinValue || value == DateTime.MaxValue ? null : (object)value;
		}
	}
}