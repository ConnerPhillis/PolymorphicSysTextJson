using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using PolymorphicSysTextJson.Exceptions;
using PolymorphicSysTextJson.Extensions;
using PolymorphicSysTextJson.Tests.TypeHierarchy;

namespace PolymorphicSysTextJson.Tests;

public class GenericTypeHierarchyTests
{
	[Fact]
	public void TestSerializeGenericTypeA()
	{
		var serializerOptions = SetupOptions();

		var instance = new GenericTypeDerivedA<int>();

		var json = JsonSerializer.Serialize<IGenericTypeRoot<int>>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator":"GenericTypeDerivedA"
			""",
			json);
	}

	[Fact]
	public void TestSerializeGenericTypeB()
	{
		var serializerOptions = SetupOptions();

		var instance = new GenericTypeDerivedB<int>();

		var json = JsonSerializer.Serialize<IGenericTypeRoot<int>>(instance, serializerOptions);

		Assert.Contains(
			"""
			"Discriminator":"GenericTypeDerivedB"
			""",
			json);
	}

	[Fact]
	public void TestSerializeGenericTypeC()
	{
		var serializerOptions = SetupOptions();

		var instance = new GenericTypeDerivedC<int>();

		Assert.Throws<ArgumentException>(
			() => { JsonSerializer.Serialize<IGenericTypeRoot<int>>(instance, serializerOptions); });
	}

	[Fact]
	public void TestDeserializeGenericTypeA()
	{
		var serializerOptions = SetupOptions();

		var json = """
			{
				"Discriminator":"GenericTypeDerivedA",
				"PropertyB":1
			}
			""";
		var instance = JsonSerializer.Deserialize<IGenericTypeRoot<int>>(json, serializerOptions);

		Assert.IsType<GenericTypeDerivedA<int>>(instance);
		Assert.Equal(0, ((GenericTypeDerivedA<int>)instance).PropertyA);
	}

	[Fact]
	public void TestDeserializeGenericTypeB()
	{
		var serializerOptions = SetupOptions();

		var json = """
			{
				"Discriminator":"GenericTypeDerivedB",
				"PropertyB":1
			}
			""";
		var instance = JsonSerializer.Deserialize<IGenericTypeRoot<int>>(json, serializerOptions);

		Assert.IsType<GenericTypeDerivedB<int>>(instance);
		Assert.Equal(1, ((GenericTypeDerivedB<int>)instance).PropertyB);
	}

	[Fact]
	public void TestDeserializeGenericTypeC()
	{
		var serializerOptions = SetupOptions();

		var json = """
			{
				"Discriminator":"GenericTypeDerivedC",
				"PropertyB":1
			}
			""";

		Assert.Throws<PolymorphicJsonException>(
			() => { JsonSerializer.Deserialize<IGenericTypeRoot<int>>(json, serializerOptions); });
	}

	[Fact]
	public void SetupOpenGenericOptions()
	{
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver()
		};

		Assert.Throws<InvalidOperationException>(
			() =>
			{
				serializerOptions.AddPolymorphicConverter(
					setup =>
					{
						setup.CreatePropertyDiscriminatedTypeMap(
								nameof(IGenericTypeRoot<int>.Discriminator),
								typeof(IGenericTypeRoot<>))
							.AddType(nameof(GenericTypeDerivedA<int>), typeof(GenericTypeDerivedA<>))
							.AddType(nameof(GenericTypeDerivedB<int>), typeof(GenericTypeDerivedB<>));
					});
			});
	}

	private JsonSerializerOptions SetupOptions()
	{
		var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
		{
			TypeInfoResolver = new DefaultJsonTypeInfoResolver()
		};

		serializerOptions.AddPolymorphicConverter(
			setup =>
			{
				setup.CreatePropertyDiscriminatedTypeMap<IGenericTypeRoot<int>>(
						nameof(IGenericTypeRoot<int>.Discriminator))
					.AddType<GenericTypeDerivedA<int>>(nameof(GenericTypeDerivedA<int>))
					.AddType<GenericTypeDerivedB<int>>(nameof(GenericTypeDerivedB<int>));
			});

		return serializerOptions;
	}
}