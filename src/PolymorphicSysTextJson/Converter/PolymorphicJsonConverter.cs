using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using PolymorphicSysTextJson.Exceptions;

namespace PolymorphicSysTextJson.Converter;

internal class PolymorphicJsonConverter<T> : JsonConverter<T>
{
	private readonly IPolymorphicTypeMap _typeMap;

	public PolymorphicJsonConverter(IPolymorphicTypeMap typeMap)
	{
		_typeMap = typeMap;
	}

	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var readerCopy = reader;

		var discriminatorPropertyName =
			options.PropertyNamingPolicy?.ConvertName(_typeMap.GetDiscriminatorPropertyName())
			?? _typeMap.GetDiscriminatorPropertyName();

		while (readerCopy.Read())
		{
			if (readerCopy.TokenType != JsonTokenType.PropertyName)
				continue;

			var currentProperty = readerCopy.GetString();
			if (currentProperty != discriminatorPropertyName)
				continue;

			readerCopy.Read();
			var discriminatorValue = readerCopy.GetString();

			if (discriminatorValue is null)
				throw new PolymorphicJsonException("Discriminator value is null");

			var mappedType = _typeMap.GetMappedType(discriminatorValue);

			if (mappedType is null)
				throw new PolymorphicJsonException("No mapped type found for discriminator value");

			var converter = options.GetConverter(mappedType);
			if (converter is null)
				throw new PolymorphicJsonException("No converter found for mapped type");

			return (T?)JsonSerializer.Deserialize(ref reader, mappedType, options);
		}

		/*
		 * we have gotten to a point where we do not have a type to deserialize
		 * therefore, we have to throw as trying to deserialize the root will result in this method
		 * getting called recursively infinitely.
		 */
		throw new PolymorphicJsonException($"No derived type found to deserialize from base type '{typeof(T)}'");
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNullValue();
			return;
		}

		var valueType = value.GetType();
		_typeMap.GetTypeDiscriminator(valueType);

		var typeInfo = options.GetTypeInfo(valueType);

		writer.WriteStartObject();

		if (_typeMap.MustWriteTypeDiscriminator())
		{
			var discriminatorPropertyName =
				options.PropertyNamingPolicy?.ConvertName(_typeMap.GetDiscriminatorPropertyName())
				?? _typeMap.GetDiscriminatorPropertyName();

			writer.WritePropertyName(discriminatorPropertyName);
			writer.WriteStringValue(_typeMap.GetTypeDiscriminator(valueType));
		}

		foreach (var property in typeInfo.Properties)
		{
			if (property.Get is null)
				continue;

			var propertyName = options.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;

			writer.WritePropertyName(propertyName);
			var propertyValue = property.Get(value);

			if (propertyValue is null)
			{
				writer.WriteNullValue();
				continue;
			}

			JsonSerializer.Serialize(writer, propertyValue, propertyValue.GetType(), options);
		}

		writer.WriteEndObject();
	}
}