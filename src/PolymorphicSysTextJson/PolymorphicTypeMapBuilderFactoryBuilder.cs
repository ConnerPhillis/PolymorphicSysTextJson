using System.Text.Json;
using System.Text.Json.Serialization;

using PolymorphicSysTextJson.Converter;

namespace PolymorphicSysTextJson;

public class PolymorphicTypeMapBuilderFactoryBuilder : IPolymorphicJsonConverterFactoryBuilder
{
	private readonly JsonSerializerOptions _jsonSerializerOptions;
	private readonly Dictionary<Type, IPolymorphicTypeMapBuilder> _typeMaps = new();

	public PolymorphicTypeMapBuilderFactoryBuilder(JsonSerializerOptions jsonSerializerOptions)
	{
		_jsonSerializerOptions = jsonSerializerOptions;
	}

	private IPolymorphicTypeMapBuilder CreatePropertyDiscriminatedTypeMap(
		string propertyName,
		Type rootType,
		bool useInternalProperty)
	{
		AssertTypeNotAlreadyAdded(rootType);
		AssertRootTypeNotOpenGeneric(rootType);

		var typeMapBuilder = new PolymorphicTypeMapBuilder(
			_jsonSerializerOptions,
			propertyName,
			rootType,
			useInternalProperty);
		_typeMaps[rootType] = typeMapBuilder;

		return typeMapBuilder;
	}

	public IPolymorphicTypeMapBuilder CreatePropertyDiscriminatedTypeMap(
		string propertyName,
		Type rootType)
		=> CreatePropertyDiscriminatedTypeMap(propertyName, rootType, true);

	public IPolymorphicTypeMapBuilder CreateMetadataDiscriminatedTypeMap<T>(
		Type rootType,
		string propertyName = "$t")
		=> CreatePropertyDiscriminatedTypeMap(propertyName, rootType, false);

	private PolymorphicTypeMapBuilder<T> CreatePropertyDiscriminatedTypeMap<T>(
		string propertyName, bool useInternalProperty)
	{
		AssertTypeNotAlreadyAdded(typeof(T));

		var typeMapBuilder = new PolymorphicTypeMapBuilder<T>(
			_jsonSerializerOptions,
			propertyName,
			useInternalProperty);
		_typeMaps[typeof(T)] = typeMapBuilder;

		return typeMapBuilder;
	}

	public IPolymorphicTypeMapBuilder<T> CreatePropertyDiscriminatedTypeMap<T>(string propertyName)
		=> CreatePropertyDiscriminatedTypeMap<T>(propertyName, true);

	public IPolymorphicTypeMapBuilder<T> CreateMetadataDiscriminatedTypeMap<T>(
		string propertyName = "$t")
		=> CreatePropertyDiscriminatedTypeMap<T>(propertyName, false);

	public JsonConverterFactory CreateConverter()
	{
		var converterFactory = new PolymorphicJsonConverterFactory();
		foreach (var (_, value) in _typeMaps)
		{
			var polymorphicTypeMap = value.Build();
			converterFactory.AddTypeMap(polymorphicTypeMap);
		}

		return converterFactory;
	}

	private void AssertTypeNotAlreadyAdded(Type type)
	{
		if (_typeMaps.ContainsKey(type))
			throw new InvalidOperationException($"type map already exists for this type '{type}'");
	}

	private void AssertRootTypeNotOpenGeneric(Type type)
	{
		if (type.IsGenericTypeDefinition)
			throw new InvalidOperationException($"root type '{type}' cannot be an open generic type");
	}
}