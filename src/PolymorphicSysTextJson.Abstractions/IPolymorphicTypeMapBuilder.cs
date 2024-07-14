namespace PolymorphicSysTextJson;

public interface IPolymorphicTypeMapBuilder
{
	public IPolymorphicTypeMapBuilder AddType(string discriminatorValue, Type type);

	public IPolymorphicTypeMap Build();
}

public interface IPolymorphicTypeMapBuilder<in T> : IPolymorphicTypeMapBuilder
{
	public IPolymorphicTypeMapBuilder<T> AddType<TChild>(string discriminatorValue)
		where TChild : T;
}