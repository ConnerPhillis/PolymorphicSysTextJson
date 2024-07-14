namespace PolymorphicSysTextJson.Tests.TypeHierarchy;

public interface IRootInterface
{
	string Discriminator { get; }
}

public class InterfaceDerivedClassA : IRootInterface
{
	public string Discriminator => nameof(InterfaceDerivedClassA);

	public string PropertyA { get; set; } = "TestA";
}

public class InterfaceDerivedClassB : IRootInterface
{
	public string Discriminator => nameof(InterfaceDerivedClassB);

	public string PropertyB { get; set; } = "TestB";
}

public class InterfaceDerivedClassC : IRootInterface
{
	public string Discriminator => nameof(InterfaceDerivedClassC);

	public string PropertyC { get; set; } = "TestC";
}
