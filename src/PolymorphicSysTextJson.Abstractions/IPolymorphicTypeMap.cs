namespace PolymorphicSysTextJson;

public interface IPolymorphicTypeMap
{
	public string GetDiscriminatorPropertyName();

	public bool MustWriteTypeDiscriminator();

	public Type GetRootType();

	public Type? GetMappedType(string discriminatorValue);

	public string GetTypeDiscriminator(Type type);
}