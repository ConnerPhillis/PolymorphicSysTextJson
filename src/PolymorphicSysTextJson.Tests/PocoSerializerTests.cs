using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using PolymorphicSysTextJson.Converter;
using PolymorphicSysTextJson.Exceptions;
using PolymorphicSysTextJson.Tests.TypeHierarchy;

namespace PolymorphicSysTextJson.Tests;

public class PocoSerializerTests
{

	[Fact]
	public void TestSerializePocoTypeRootThrows()
	{
		var serializerOptions = GetPocoDerivedJsonOptions();

		var instance = new PocoTypeRoot();

		Assert.Throws<ArgumentException>(
			() => { JsonSerializer.Serialize(instance, serializerOptions); });
	}

	[Fact]
	public void TestSerializePocoTypeDerivedA()
	{
		var serializerOptions = GetPocoDerivedJsonOptions();

		var instance = new PocoTypeDerivedA();
		var json = JsonSerializer.Serialize<PocoTypeRoot>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator":"PocoTypeDerivedA"
			""",
			json);
	}

	[Fact]
	public void TestSerializePocoTypeDerivedB()
	{
		var serializerOptions = GetPocoDerivedJsonOptions();

		var instance = new PocoTypeDerivedB();
		var json = JsonSerializer.Serialize<PocoTypeRoot>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator":"PocoTypeDerivedB"
			""",
			json);
	}

	[Fact]
	public void TestSerializePocoTypeDerivedC()
	{
		var serializerOptions = GetPocoDerivedJsonOptions();

		var instance = new PocoTypeDerivedC();

		Assert.Throws<ArgumentException>(
			() => { JsonSerializer.Serialize<PocoTypeRoot>(instance, serializerOptions); });
	}

	[Fact]
	public void TestDeserializePocoTypeDerivedA()
	{
		var serializerOptions = GetPocoDerivedJsonOptions();

		var json = """{"Discriminator":"PocoTypeDerivedA","PropertyA":"TestA"}""";
		var instance = JsonSerializer.Deserialize<PocoTypeRoot>(json, serializerOptions);

		Assert.IsType<PocoTypeDerivedA>(instance);
		Assert.Equal("TestA", ((PocoTypeDerivedA)instance).PropertyA);
	}

	[Fact]
	public void TestDeserializePocoTypeDerivedB()
	{
		var serializerOptions = GetPocoDerivedJsonOptions();

		var json = """{"Discriminator":"PocoTypeDerivedB","PropertyB":"TestB"}""";
		var instance = JsonSerializer.Deserialize<PocoTypeRoot>(json, serializerOptions);

		Assert.IsType<PocoTypeDerivedB>(instance);
		Assert.Equal("TestB", ((PocoTypeDerivedB)instance).PropertyB);
	}

	[Fact]
	public void TestDeserializePocoTypeDerivedC()
	{
		var serializerOptions = GetPocoDerivedJsonOptions();

		var json = """{"Discriminator":"PocoTypeDerivedC","PropertyC":"TestC"}""";

		Assert.Throws<PolymorphicJsonException>(
			() => { JsonSerializer.Deserialize<PocoTypeRoot>(json, serializerOptions); });
	}

	[Fact]
	public void TestDeserializePocoTypeRootThrows()
	{
		var serializerOptions = GetPocoDerivedJsonOptions();

		var json = """{"Discriminator":"PocoTypeRoot"}""";

		Assert.Throws<PolymorphicJsonException>(
			() =>
			{
				JsonSerializer.Deserialize<PocoTypeRoot>(json, serializerOptions);
			});
	}

	private JsonSerializerOptions GetPocoDerivedJsonOptions()
	{
		var converterFactory = new PolymorphicJsonConverterFactory();

		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
			Converters = { converterFactory }
		};

		var typeMap = new PropertyDiscriminatedTypeMap(
			serializerOptions,
			nameof(PocoTypeRoot.Discriminator),
			typeof(PocoTypeRoot));
		typeMap.AddType(nameof(PocoTypeDerivedA), typeof(PocoTypeDerivedA));
		typeMap.AddType(nameof(PocoTypeDerivedB), typeof(PocoTypeDerivedB));

		converterFactory.AddTypeMap(typeMap);
		return serializerOptions;
	}
}