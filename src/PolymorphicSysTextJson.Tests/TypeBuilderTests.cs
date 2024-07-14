using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using PolymorphicSysTextJson.Tests.TypeHierarchy;

namespace PolymorphicSysTextJson.Tests;

public class PropertyDiscriminatedTypeMapTests
{
	[Fact]
	public void AssertCanAddDerivedTypes()
	{
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
		};

		var typeBuilder = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			"test",
			typeof(AbstractClass));

		typeBuilder.AddType("test1", typeof(DerivedAbstractClassA));
		typeBuilder.AddType("test2", typeof(DerivedAbstractClassB));
		typeBuilder.AddType("test3", typeof(DerivedAbstractClassC));

		Assert.True(true);
	}

	[Fact]
	public void AssertNonDerivedTypesThrow()
	{
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
		};
		var typeBuilder = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			"test",
			typeof(AbstractClass));

		Assert.Throws<ArgumentException>(() => typeBuilder.AddType("test1", typeof(PropertyDiscriminatedTypeMapTests)));
	}

	[Fact]
	public void AssertAbstractRootTypeAddedThrows()
	{
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
		};
		var typeBuilder = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			"test",
			typeof(AbstractClass));

		Assert.Throws<ArgumentException>(() => typeBuilder.AddType("test1", typeof(AbstractClass)));
	}

	[Fact]
	public void AssertInterfaceRootTypeAddedThrows()
	{
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
		};
		var typeBuilder = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			"test",
			typeof(IRootInterface));

		Assert.Throws<ArgumentException>(() => typeBuilder.AddType("test1", typeof(IRootInterface)));
	}

	[Fact]
	public void AssertPocoRootTypeAddedThrows()
	{
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
		};
		var typeBuilder = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			"test",
			typeof(PocoTypeRoot));

		Assert.Throws<ArgumentException>(() => typeBuilder.AddType("test1", typeof(PocoTypeRoot)));
	}

	[Fact]
	public void AssertDuplicateTypeThrows()
	{
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
		};
		var typeBuilder = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			"test",
			typeof(AbstractClass));

		typeBuilder.AddType("test1", typeof(DerivedAbstractClassA));

		Assert.Throws<ArgumentException>(
			() => typeBuilder.AddType("test1", typeof(DerivedAbstractClassA)));
	}

	[Fact]
	public void AssertDuplicateDiscriminatorThrows()
	{
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
		};
		var typeBuilder = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			"test",
			typeof(AbstractClass));

		typeBuilder.AddType("test1", typeof(DerivedAbstractClassA));

		Assert.Throws<ArgumentException>(
			() => typeBuilder.AddType("test1", typeof(DerivedAbstractClassB)));
	}

	[Fact]
	public void AssertNonEmbeddedPropertyNameThrows()
	{
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
		};
		var typeBuilder = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			"Discriminator",
			typeof(AbstractClass),
			false);

		Assert.Throws<ArgumentException>(
			() => typeBuilder.AddType("test", typeof(DerivedAbstractClassA)));
	}
}