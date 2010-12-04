namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;

	internal static class ExtensionMethods
	{
		public static ICollection<string> GetMatching(
			this IDictionary<Type, ICollection<string>> source, IEnumerable<Type> types)
		{
			ICollection<string> list = new HashSet<string>();

			foreach (var type in types)
			{
				ICollection<string> values;
				if (!source.TryGetValue(type, out values))
					continue;

				foreach (var value in values)
					list.Add(value);
			}

			return list;
		}
	}
}