namespace NanoMessageBus.MessageSubscriber
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal static class ExtensionMethods
	{
		public static string[] GetTypeNames(this IEnumerable<Type> types)
		{
			return types.Where(x => x != null).Select(x => x.AssemblyQualifiedName).ToArray();
		}
	}
}