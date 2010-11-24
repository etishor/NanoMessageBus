namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal static class ExtensionMethods
	{
		public static object[] PopulatedMessagesOnly(this object[] messages)
		{
			messages = messages ?? new object[] { };
			return messages.Where(x => x != null).ToArray();
		}

		public static ICollection<string> GetRecipients(
			this IDictionary<Type, IEnumerable<string>> registrations, IEnumerable<Type> messageTypes)
		{
			ICollection<string> recipients = new HashSet<string>();

			foreach (var messageType in messageTypes)
			{
				IEnumerable<string> recipientsForMessageType;
				if (!registrations.TryGetValue(messageType, out recipientsForMessageType))
					continue;

				foreach (var recipient in recipientsForMessageType)
					recipients.Add(recipient);
			}

			return recipients;
		}

		public static IEnumerable<Type> GetTypes(this object primaryMessage)
		{
			ICollection<Type> list = new HashSet<Type>();
			if (primaryMessage == null)
				return list;

			var messageType = primaryMessage.GetType();
			list.Add(messageType);
			foreach (var @interface in messageType.GetInterfaces())
				list.Add(@interface);

			return list;
		}
	}
}