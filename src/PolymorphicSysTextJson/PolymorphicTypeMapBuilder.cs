using System.Text.Json;

namespace PolymorphicSysTextJson;

public class PolymorphicTypeMapBuilder : IPolymorphicTypeMapBuilder
{

	protected readonly PropertyDiscriminatedTypeMap PropertyDiscriminatedTypeMap;

	public PolymorphicTypeMapBuilder(JsonSerializerOptions jsonSerializerOptions, string discriminatorProperty, Type rootType, bool useEmbeddedProperty)
	{
		PropertyDiscriminatedTypeMap = new PropertyDiscriminatedTypeMap(
			jsonSerializerOptions,
			discriminatorProperty,
			rootType,
			useEmbeddedProperty);
	}

	public IPolymorphicTypeMapBuilder AddType(string discriminatorValue, Type type)
	{
		PropertyDiscriminatedTypeMap.AddType(discriminatorValue, type);
		return this;
	}

	public IPolymorphicTypeMap Build()
		=> PropertyDiscriminatedTypeMap;
}

public class PolymorphicTypeMapBuilder<T> : PolymorphicTypeMapBuilder, IPolymorphicTypeMapBuilder<T>
{
	public PolymorphicTypeMapBuilder(JsonSerializerOptions jsonSerializerOptions, string discriminatorProperty, bool useEmbeddedProperty)
		: base(jsonSerializerOptions, discriminatorProperty, typeof(T), useEmbeddedProperty)
	{
	}

	public IPolymorphicTypeMapBuilder<T> AddType<TChild>(string discriminatorValue)
		where TChild : T
	{
		PropertyDiscriminatedTypeMap.AddType(discriminatorValue, typeof(TChild));
		return this;
	}
}