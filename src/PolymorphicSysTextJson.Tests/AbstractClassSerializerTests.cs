using PolymorphicSysTextJson.Converter;
using PolymorphicSysTextJson.Exceptions;
using PolymorphicSysTextJson.Tests.TypeHierarchy;

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace PolymorphicSysTextJson.Tests;

public class AbstractClassSerializerTests
{
	[Fact]
	public void TestSerializeAbstractDerivedClassA()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptions();

		var instance = new DerivedAbstractClassA();
		var json = JsonSerializer.Serialize<AbstractClass>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator":"DerivedAbstractClassA"
			""",
			json);
	}

	[Fact]
	public void TestSerializeAbstractDerivedClassB()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptions();

		var instance = new DerivedAbstractClassB();
		var json = JsonSerializer.Serialize<AbstractClass>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator":"DerivedAbstractClassB"
			""",
			json);
	}

	[Fact]
	public void TestSerializeAbstractDerivedClassC()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptions();

		var instance = new DerivedAbstractClassC();

		Assert.Throws<ArgumentException>(
			() => { JsonSerializer.Serialize<AbstractClass>(instance, serializerOptions); });
	}

	[Fact]
	public void TestDeserializeAbstractDerivedClassA()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptions();

		var json = """
					{
						"Discriminator":"DerivedAbstractClassA",
						"PropertyA":"TestA"
					}
			""";

		var instance = JsonSerializer.Deserialize<AbstractClass>(json, serializerOptions);

		Assert.IsType<DerivedAbstractClassA>(instance);
		Assert.Equal("TestA", ((DerivedAbstractClassA)instance).PropertyA);
	}

	[Fact]
	public void TestDeserializeAbstractDerivedClassB()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptions();

		var json = """
					{
						"Discriminator":"DerivedAbstractClassB",
						"PropertyB":"TestB"
					}
			""";

		var instance = JsonSerializer.Deserialize<AbstractClass>(json, serializerOptions);

		Assert.IsType<DerivedAbstractClassB>(instance);
		Assert.Equal("TestB", ((DerivedAbstractClassB)instance).PropertyB);
	}

	[Fact]
	public void TestDeserializeAbstractDerivedClassC()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptions();

		var json = """
					{
						"Discriminator":"InterfaceDerivedClassC",
						"PropertyC":"TestC"
					}
			""";

		Assert.Throws<PolymorphicJsonException>(
			() => { JsonSerializer.Deserialize<AbstractClass>(json, serializerOptions); });
	}

	[Fact]
	public void TestSerializeAbstractDerivedClassAWithExternalProperties()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptionsWithExternalProperties();

		var instance = new DerivedAbstractClassA();
		var json = JsonSerializer.Serialize<AbstractClass>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator_External":"DerivedAbstractClassA"
			""",
			json);
	}

	[Fact]
	public void TestSerializeAbstractDerivedClassBWithExternalProperties()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptionsWithExternalProperties();

		var instance = new DerivedAbstractClassB();
		var json = JsonSerializer.Serialize<AbstractClass>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator_External":"DerivedAbstractClassB"
			""",
			json);
	}

	[Fact]
	public void TestSerializeAbstractDerivedClassCWithExternalProperties()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptionsWithExternalProperties();

		var instance = new DerivedAbstractClassC();

		Assert.Throws<ArgumentException>(
			() => { JsonSerializer.Serialize<AbstractClass>(instance, serializerOptions); });
	}

	[Fact]
	public void TestDeserializeAbstractDerivedClassAWithExternalProperties()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptionsWithExternalProperties();

		var json = """
					{
						"Discriminator_External":"DerivedAbstractClassA",
						"PropertyA":"TestA"
					}
			""";

		var instance = JsonSerializer.Deserialize<AbstractClass>(json, serializerOptions);

		Assert.IsType<DerivedAbstractClassA>(instance);
		Assert.Equal("TestA", ((DerivedAbstractClassA)instance).PropertyA);
	}

	[Fact]
	public void TestDeserializeAbstractDerivedClassBWithExternalProperties()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptionsWithExternalProperties();

		var json = """
					{
						"Discriminator_External":"DerivedAbstractClassB",
						"PropertyB":"TestB"
					}
			""";

		var instance = JsonSerializer.Deserialize<AbstractClass>(json, serializerOptions);

		Assert.IsType<DerivedAbstractClassB>(instance);
		Assert.Equal("TestB", ((DerivedAbstractClassB)instance).PropertyB);
	}

	[Fact]
	public void TestDeserializeAbstractDerivedClassCWithExternalProperties()
	{
		var serializerOptions = GetAbstractClassDerivedJsonOptionsWithExternalProperties();

		var json = """
					{
						"Discriminator_External":"InterfaceDerivedClassC",
						"PropertyC":"TestC"
					}
			""";

		Assert.Throws<PolymorphicJsonException>(
			() => { JsonSerializer.Deserialize<AbstractClass>(json, serializerOptions); });
	}

	private JsonSerializerOptions GetAbstractClassDerivedJsonOptions()
	{
		var converterFactory = new PolymorphicJsonConverterFactory();
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
			Converters = { converterFactory }
		};

		var typeMap = new PropertyDiscriminatedTypeMap(
			JsonSerializerOptions.Default,
			nameof(AbstractClass.Discriminator),
			typeof(AbstractClass));

		typeMap.AddType(nameof(DerivedAbstractClassA), typeof(DerivedAbstractClassA));
		typeMap.AddType(nameof(DerivedAbstractClassB), typeof(DerivedAbstractClassB));
		converterFactory.AddTypeMap(typeMap);

		return serializerOptions;
	}

	private JsonSerializerOptions GetAbstractClassDerivedJsonOptionsWithExternalProperties()
	{
		var converterFactory = new PolymorphicJsonConverterFactory();
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
			Converters = { converterFactory }
		};

		var typeMap = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			nameof(AbstractClass.Discriminator) + "_External",
			typeof(AbstractClass),
			false);

		typeMap.AddType(nameof(DerivedAbstractClassA), typeof(DerivedAbstractClassA));
		typeMap.AddType(nameof(DerivedAbstractClassB), typeof(DerivedAbstractClassB));
		converterFactory.AddTypeMap(typeMap);

		return serializerOptions;
	}
}