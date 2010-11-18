namespace NanoMessageBus
{
	using System.Collections.Generic;
	using System.Linq;

	internal static class ExtensionMethods
	{
		public static bool HasMessages(this IEnumerable<object> messages)
		{
			return null != messages && messages.Where(x => x != null).Any();
		}
	}
}