namespace NanoMessageBus.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MessageTypeDiscoverer : IDiscoverMessageTypes
	{
		public virtual IEnumerable<Type> GetTypes(object message)
		{
			if (message != null)
			{
				var messageType = message.GetType();
				yield return messageType;

				foreach (var @interface in messageType.GetInterfaces())
					yield return @interface;
			}
		}
		public virtual IEnumerable<string> GetTypeNames(object message)
		{
			return this.GetTypes(message).Select(type => type.AssemblyQualifiedName);
		}
	}
}