namespace PolymorphicSysTextJson.Tests.TypeHierarchy;

public interface IGenericTypeRoot<T>
{
	public string Discriminator { get; }
}

public class GenericTypeDerivedA<T> : IGenericTypeRoot<T>
{
	public string Discriminator => nameof(GenericTypeDerivedA<T>);

	public T PropertyA { get; set; }
}

public class GenericTypeDerivedB<T> : IGenericTypeRoot<T>
{
	public string Discriminator => nameof(GenericTypeDerivedB<T>);

	public T PropertyB { get; set; }
}

public class GenericTypeDerivedC<T> : IGenericTypeRoot<T>
{
	public string Discriminator => nameof(GenericTypeDerivedC<T>);

	public T PropertyC { get; set; }
}