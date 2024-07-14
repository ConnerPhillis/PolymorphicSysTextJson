using System.Text.Json;

namespace PolymorphicSysTextJson.Exceptions;

public class PolymorphicJsonException : JsonException
{
	public PolymorphicJsonException(string message) : base(message)
	{
	}

	public PolymorphicJsonException(string message, Exception innerException) : base(message, innerException)
	{
	}
}