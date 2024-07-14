using System.Text.Json.Serialization;

namespace PolymorphicSysTextJson;

public interface IPolymorphicJsonConverterFactoryBuilder
{
	public IPolymorphicTypeMapBuilder CreatePropertyDiscriminatedTypeMap(
		string propertyName,
		Type rootType);

	public IPolymorphicTypeMapBuilder CreateMetadataDiscriminatedTypeMap<T>(
		Type rootType,
		string propertyName = "$t");

	public IPolymorphicTypeMapBuilder<T> CreatePropertyDiscriminatedTypeMap<T>(string propertyName);

	public IPolymorphicTypeMapBuilder<T> CreateMetadataDiscriminatedTypeMap<T>(
		string propertyName = "$t");


	public JsonConverterFactory CreateConverter();
}