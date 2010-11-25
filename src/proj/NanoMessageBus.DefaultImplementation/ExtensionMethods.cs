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

		public static IEnumerable<string> GetTypeNames(this IEnumerable<Type> types)
		{
			return types.Select(type => type.AssemblyQualifiedName);
		}
	}
}