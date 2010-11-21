namespace NanoMessageBus.SubscriptionStorage
{
	using System;

	public static class ExtensionMethods
	{
		public static object ToNull(DateTime value)
		{
			return null; // dt min/max means null
		}
	}
}