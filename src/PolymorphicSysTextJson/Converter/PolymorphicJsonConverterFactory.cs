using System.Text.Json;
using System.Text.Json.Serialization;

using PolymorphicSysTextJson.Exceptions;

namespace PolymorphicSysTextJson.Converter;

public class PolymorphicJsonConverterFactory : JsonConverterFactory
{

	internal Dictionary<Type, IPolymorphicTypeMap> TypeMaps { get; } = new();

	public void AddTypeMap(IPolymorphicTypeMap typeMap)
		=> TypeMaps[typeMap.GetRootType()] = typeMap;

	public override bool CanConvert(Type typeToConvert)
		=> GetMappedType(typeToConvert) is not null;

	public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		var typeMap = GetMappedType(typeToConvert);
		if (typeMap is null)
			throw new PolymorphicJsonException("no type map was registered for this type");

		// create an instance of polymorphic json converter, instantiate it with the correct generic
		// type argument and pass the type map to it
		var converterType = typeof(PolymorphicJsonConverter<>).MakeGenericType(typeToConvert);
		if (converterType is null)
			throw new InvalidOperationException("how did this happen");

		return (JsonConverter?)Activator.CreateInstance(converterType, typeMap);
	}

	private IPolymorphicTypeMap? GetMappedType(Type type)
		=> TypeMaps.GetValueOrDefault(type);
}