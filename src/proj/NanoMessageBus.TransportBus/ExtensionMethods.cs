namespace NanoMessageBus
{
	using System;
	using System.Collections.Generic;

	internal static class ExtensionMethods
	{
		public static ICollection<Uri> GetMatching(
			this IDictionary<Type, ICollection<Uri>> source, IEnumerable<Type> types)
		{
			ICollection<Uri> list = new HashSet<Uri>();

			foreach (var type in types)
			{
				ICollection<Uri> values;
				if (!source.TryGetValue(type, out values))
					continue;

				foreach (var value in values)
					list.Add(value);
			}

			return list;
		}
	}
}