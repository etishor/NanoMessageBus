namespace NanoMessageBus.SubscriptionStorage
{
	using System.Data;

	internal static class CommandBuilder
	{
		public static void AddParameter(this IDbCommand command, string parameterName, object parameterValue)
		{
			var parameter = command.CreateParameter();
			parameter.ParameterName = parameterName;
			parameter.Value = parameterValue;
			command.Parameters.Add(parameter);
		}
	}
}