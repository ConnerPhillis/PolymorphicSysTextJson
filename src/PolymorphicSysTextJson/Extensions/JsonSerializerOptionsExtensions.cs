using System.Text.Json;

namespace PolymorphicSysTextJson.Extensions;

public static class JsonSerializerOptionsExtensions
{
	public static JsonSerializerOptions AddPolymorphicConverter(
		this JsonSerializerOptions options,
		Action<IPolymorphicJsonConverterFactoryBuilder> configurePolymorphicJson)
	{
		var builder = new PolymorphicTypeMapBuilderFactoryBuilder(options);

		configurePolymorphicJson(builder);

		var converter = builder.CreateConverter();

		options.Converters.Add(converter);
		return options;
	}
}