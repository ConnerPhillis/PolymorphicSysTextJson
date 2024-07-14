namespace PolymorphicSysTextJson.Tests.TypeHierarchy;

abstract class AbstractClass
{
	public abstract string Discriminator { get; }
}

class DerivedAbstractClassA : AbstractClass
{
	public override string Discriminator => nameof(DerivedAbstractClassA);

	public string PropertyA { get; set; } = "TestA";
}

class DerivedAbstractClassB : AbstractClass
{
	public override string Discriminator => nameof(DerivedAbstractClassB);

	public string PropertyB { get; set; } = "TestB";
}

class DerivedAbstractClassC : AbstractClass
{
	public override string Discriminator => nameof(DerivedAbstractClassC);

	public string PropertyC { get; set; } = "TestC";
}