using System.Text.Json;

namespace PolymorphicSysTextJson;

public class PropertyDiscriminatedTypeMap : IPolymorphicTypeMap
{
	private readonly JsonSerializerOptions _jsonSerializerOptions;
	private readonly Type _rootType;
	private readonly bool _useEmbeddedEmbeddedProperty;
	private readonly string _discriminatorPropertyName;

	private Lazy<bool> _requiresNewTypeMap;

	private Dictionary<string, Type> TypeMap { get; } = new();

	public PropertyDiscriminatedTypeMap(
		JsonSerializerOptions jsonSerializerOptions,
		string discriminatorPropertyName,
		Type rootType,
		bool useEmbeddedProperty = true)
	{
		_jsonSerializerOptions = jsonSerializerOptions;
		_rootType = rootType;
		_useEmbeddedEmbeddedProperty = useEmbeddedProperty;
		_discriminatorPropertyName = discriminatorPropertyName;

		_requiresNewTypeMap = new Lazy<bool>(() => _rootType.IsGenericTypeDefinition);
	}

	private PropertyDiscriminatedTypeMap(
		JsonSerializerOptions jsonSerializerOptions,
		string discriminatorPropertyName,
		Type rootType,
		bool useEmbeddedProperty,
		Dictionary<string, Type> typeMap) : this(jsonSerializerOptions, discriminatorPropertyName, rootType, useEmbeddedProperty)
	{
		TypeMap = typeMap;
	}


	public string GetDiscriminatorPropertyName()
		=> _discriminatorPropertyName;

	public bool MustWriteTypeDiscriminator()
		=> !_useEmbeddedEmbeddedProperty;

	public Type GetRootType()
		=> _rootType;

	public Type? GetMappedType(string discriminatorValue)
		=> TypeMap.GetValueOrDefault(discriminatorValue);

	public string GetTypeDiscriminator(Type type)
	{
		foreach (var (discriminator, mappedType) in TypeMap)
			if (mappedType == type)
				return discriminator;

		throw new ArgumentException("Type not found in type map", nameof(type));
	}

	public bool RequiresNewTypeMap()
		=> _requiresNewTypeMap.Value;

	public void AddType(string discriminatorValue, Type type)
	{
		AssertTypeIsChildOfRoot(type);
		AssertTypeIsNotRoot(type);
		AssertTypeIsNotRegistered(type);
		AssertDiscriminatorIsNotRegistered(discriminatorValue);
		if (!_useEmbeddedEmbeddedProperty)
			AssertTypeDoesNotContainPropertyName(type);

		TypeMap[discriminatorValue] = type;
	}

	private void AssertTypeIsChildOfRoot(Type type)
	{
		if (type.GetInterface(_rootType.Name) is not null)
			return;

		if (!_rootType.IsAssignableFrom(type))
			throw new ArgumentException(
				$"Type '{type}' is not a child of the root type '{_rootType}'",
				nameof(type));
	}

	private void AssertTypeIsNotRoot(Type type)
	{
		if (_rootType == type)
			throw new ArgumentException($"Type '{type}' is the root type", nameof(type));
	}

	private void AssertTypeIsNotRegistered(Type type)
	{
		if (TypeMap.Values.Contains(type))
			throw new ArgumentException($"Type '{type}' is already registered", nameof(type));
	}

	private void AssertDiscriminatorIsNotRegistered(string discriminatorValue)
	{
		if (TypeMap.ContainsKey(discriminatorValue))
			throw new ArgumentException(
				$"Discriminator '{discriminatorValue}' is already registered",
				nameof(discriminatorValue));
	}

	private void AssertTypeDoesNotContainPropertyName(Type type)
	{
		// we only have to use the default options here

		var typeInfo = _jsonSerializerOptions.GetTypeInfo(type);

		var convertedDiscriminatorPropertyName =
			_jsonSerializerOptions.PropertyNamingPolicy?.ConvertName(_discriminatorPropertyName)
			?? _discriminatorPropertyName;

		foreach (var property in typeInfo.Properties)
		{
			var propertyName = _jsonSerializerOptions.PropertyNamingPolicy?.ConvertName(property.Name)
				?? property.Name;
			if (propertyName.Equals(convertedDiscriminatorPropertyName))
				throw new ArgumentException(
					$"Type '{type}' contains a match for discriminator named '{_discriminatorPropertyName}' after naming policy application",
					nameof(type));
		}
	}
}