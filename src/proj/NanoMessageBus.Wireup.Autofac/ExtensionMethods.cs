namespace NanoMessageBus.Wireup
{
	using System;
	using System.Collections.Generic;

	public static class ExtensionMethods
	{
		public static int Threads(this int threads)
		{
			return threads;
		}
		public static int Times(this int times)
		{
			return times;
		}

		internal static TWireup Add<TWireup>(this IDictionary<Type, IWireup> modules, TWireup module)
			where TWireup : class, IWireup
		{
			if (module == null)
				modules.Remove(typeof(TWireup));
			else
				modules[typeof(TWireup)] = module;

			return module;
		}
	}
}