using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using PolymorphicSysTextJson.Converter;
using PolymorphicSysTextJson.Exceptions;
using PolymorphicSysTextJson.Tests.TypeHierarchy;

namespace PolymorphicSysTextJson.Tests;

public class InterfaceSerializerTests
{
	[Fact]
	public void TestSerializeInterfacedDerivedClassA()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptions();

		var instance = new InterfaceDerivedClassA();
		var json = JsonSerializer.Serialize<IRootInterface>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator":"InterfaceDerivedClassA"
			""",
			json);
	}

	[Fact]
	public void TestSerializeInterfacedDerivedClassB()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptions();

		var instance = new InterfaceDerivedClassB();
		var json = JsonSerializer.Serialize<IRootInterface>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator":"InterfaceDerivedClassB"
			""",
			json);
	}

	[Fact]
	public void TestSerializeInterfacedDerivedClassC()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptions();

		var instance = new InterfaceDerivedClassC();

		Assert.Throws<ArgumentException>(
			() => { JsonSerializer.Serialize<IRootInterface>(instance, serializerOptions); });
	}

	[Fact]
	public void TestDeserializeInterfacedDerivedClassA()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptions();

		var json = """{"Discriminator":"InterfaceDerivedClassA","PropertyB":"TestA"}""";
		var instance = JsonSerializer.Deserialize<IRootInterface>(json, serializerOptions);

		Assert.IsType<InterfaceDerivedClassA>(instance);
		Assert.Equal("TestA", ((InterfaceDerivedClassA)instance).PropertyA);
	}

	[Fact]
	public void TestDeserializeInterfacedDerivedClassB()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptions();

		var json = """{"Discriminator":"InterfaceDerivedClassB","PropertyB":"TestB"}""";
		var instance = JsonSerializer.Deserialize<IRootInterface>(json, serializerOptions);

		Assert.IsType<InterfaceDerivedClassB>(instance);
		Assert.Equal("TestB", ((InterfaceDerivedClassB)instance).PropertyB);
	}

	[Fact]
	public void TestDeserializeInterfacedDerivedClassC()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptions();

		var json = """{"Discriminator":"InterfaceDerivedClassC","PropertyC":"TestC"}""";
		Assert.Throws<PolymorphicJsonException>(
			() => { JsonSerializer.Deserialize<IRootInterface>(json, serializerOptions); });
	}

	[Fact]
	public void TestSerializeInterfacedDerivedClassAWithExternalProperties()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptionsWithExternalProperties();

		var instance = new InterfaceDerivedClassA();
		var json = JsonSerializer.Serialize<IRootInterface>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator_External":"InterfaceDerivedClassA"
			""",
			json);
	}

	[Fact]
	public void TestSerializeInterfacedDerivedClassBWithExternalProperties()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptionsWithExternalProperties();

		var instance = new InterfaceDerivedClassB();
		var json = JsonSerializer.Serialize<IRootInterface>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator_External":"InterfaceDerivedClassB"
			""",
			json);
	}

	[Fact]
	public void TestSerializeInterfacedDerivedClassCWithExternalProperties()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptionsWithExternalProperties();

		var instance = new InterfaceDerivedClassC();

		Assert.Throws<ArgumentException>(
			() => { JsonSerializer.Serialize<IRootInterface>(instance, serializerOptions); });
	}

	[Fact]
	public void TestDeserializeInterfacedDerivedClassAWithExternalProperties()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptionsWithExternalProperties();

		var json = """{"Discriminator_External":"InterfaceDerivedClassA","PropertyB":"TestA"}""";
		var instance = JsonSerializer.Deserialize<IRootInterface>(json, serializerOptions);

		Assert.IsType<InterfaceDerivedClassA>(instance);
		Assert.Equal("TestA", ((InterfaceDerivedClassA)instance).PropertyA);
	}

	[Fact]
	public void TestDeserializeInterfacedDerivedClassBWithExternalProperties()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptionsWithExternalProperties();

		var json = """{"Discriminator_External":"InterfaceDerivedClassB","PropertyB":"TestB"}""";
		var instance = JsonSerializer.Deserialize<IRootInterface>(json, serializerOptions);

		Assert.IsType<InterfaceDerivedClassB>(instance);
		Assert.Equal("TestB", ((InterfaceDerivedClassB)instance).PropertyB);
	}

	[Fact]
	public void TestDeserializeInterfacedDerivedClassCWithExternalProperties()
	{
		var serializerOptions = GetInterfaceDerivedJsonOptionsWithExternalProperties();

		var json = """{"Discriminator_External":"InterfaceDerivedClassC","PropertyC":"TestC"}""";
		Assert.Throws<PolymorphicJsonException>(
			() => { JsonSerializer.Deserialize<IRootInterface>(json, serializerOptions); });
	}


	private JsonSerializerOptions GetInterfaceDerivedJsonOptions()
	{
		var converterFactory = new PolymorphicJsonConverterFactory();
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
			Converters = { converterFactory }
		};

		var typeMap = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			nameof(IRootInterface.Discriminator),
			typeof(IRootInterface));
		typeMap.AddType(nameof(InterfaceDerivedClassA), typeof(InterfaceDerivedClassA));
		typeMap.AddType(nameof(InterfaceDerivedClassB), typeof(InterfaceDerivedClassB));
		converterFactory.AddTypeMap(typeMap);

		return serializerOptions;
	}

	private JsonSerializerOptions GetInterfaceDerivedJsonOptionsWithExternalProperties()
	{
		var converterFactory = new PolymorphicJsonConverterFactory();
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
			Converters = { converterFactory }
		};

		var typeMap = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			nameof(IRootInterface.Discriminator) + "_External",
			typeof(IRootInterface),
			false);

		typeMap.AddType(nameof(InterfaceDerivedClassA), typeof(InterfaceDerivedClassA));
		typeMap.AddType(nameof(InterfaceDerivedClassB), typeof(InterfaceDerivedClassB));
		converterFactory.AddTypeMap(typeMap);

		return serializerOptions;
	}
}