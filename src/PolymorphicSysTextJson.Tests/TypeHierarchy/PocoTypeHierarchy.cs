namespace PolymorphicSysTextJson.Tests.TypeHierarchy;

public class PocoTypeRoot
{
	public virtual string Discriminator { get; } = nameof(PocoTypeRoot);
}

public class PocoTypeDerivedA : PocoTypeRoot
{
	public override string Discriminator { get; } = nameof(PocoTypeDerivedA);

	public string PropertyA { get; set; } = "TestA";
}

public class PocoTypeDerivedB : PocoTypeRoot
{
	public override string Discriminator { get; } = nameof(PocoTypeDerivedB);

	public string PropertyB { get; set; } = "TestB";
}

public class PocoTypeDerivedC : PocoTypeRoot
{
	public override string Discriminator { get; } = nameof(PocoTypeDerivedC);

	public string PropertyC { get; set; } = "TestC";
}

