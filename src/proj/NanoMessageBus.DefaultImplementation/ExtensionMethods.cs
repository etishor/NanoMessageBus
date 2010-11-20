namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal static class ExtensionMethods
	{
		public static IEnumerable<object> PopulatedMessagesOnly(this IEnumerable<object> messages)
		{
			messages = messages ?? new object[] { };
			return messages.Where(x => x != null).ToList();
		}
		public static bool HasAny(this IEnumerable<object> messages)
		{
			return null != messages && messages.Where(x => x != null).Any();
		}

		public static ICollection<string> GetRecipients(
			this IDictionary<Type, IEnumerable<string>> recipients, Type primaryMessageType)
		{
			IEnumerable<string> recipientsForMessageType;
			recipients.TryGetValue(primaryMessageType, out recipientsForMessageType);
			return new List<string>(recipientsForMessageType ?? new string[] { });
		}
	}
}