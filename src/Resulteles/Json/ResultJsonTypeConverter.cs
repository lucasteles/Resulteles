using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Resulteles.Json;

/// <inheritdoc />
public class ResultJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType &&
        typeToConvert.GetGenericTypeDefinition() == typeof(Result<,>);

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var okType = typeToConvert.GetGenericArguments()[0];
        var errorType = typeToConvert.GetGenericArguments()[1];

        return Activator.CreateInstance(
            typeof(ResultJsonConverter<,>).MakeGenericType(okType, errorType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[]
            {
                options,
            },
            culture: null) as JsonConverter;
    }
}

/// <inheritdoc />
public class ResultJsonConverter<TOk, TError> : JsonConverter<Result<TOk, TError>>

{
    readonly JsonConverter<TOk> okConverter;
    readonly JsonConverter<TError> errorConverter;
    readonly Type okType = typeof(TOk);
    readonly Type errorType = typeof(TError);

    const string propertyStatusName = "status";
    const string propertyValueName = "value";
    const string statusOk = "ok";
    const string statusError = "error";

    /// <inheritdoc />
    public ResultJsonConverter(JsonSerializerOptions options)
    {
        okConverter = (JsonConverter<TOk>)options.GetConverter(okType);
        errorConverter = (JsonConverter<TError>)options.GetConverter(errorType);
    }

    /// <inheritdoc />
    public override Result<TOk, TError> Read(
        ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var json = JsonElement.ParseValue(ref reader);

        if (!json.TryGetProperty(options.PropertyNamingPolicy?.ConvertName(propertyStatusName) ??
                                 propertyStatusName, out var status))
            throw new JsonException("Invalid status property");
        if (!json.TryGetProperty(options.PropertyNamingPolicy?.ConvertName(propertyValueName) ??
                                 propertyValueName, out var value))
            throw new JsonException("Invalid value property");

        Utf8JsonReader valueReader = new(JsonSerializer.SerializeToUtf8Bytes(value));
        valueReader.Read();
        if (status.GetString() == statusOk)
            return new(
                okConverter.Read(ref valueReader, okType, options)!);

        if (status.GetString() == statusError)
            return new(
                errorConverter.Read(ref valueReader, errorType, options)!);

        throw new JsonException($"Invalid status {status}");
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer, Result<TOk, TError> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.IsOk)
        {
            writer.WritePropertyName
            (options.PropertyNamingPolicy?.ConvertName(propertyStatusName) ??
             propertyStatusName);
            writer.WriteStringValue(statusOk);

            writer.WritePropertyName
            (options.PropertyNamingPolicy?.ConvertName(propertyValueName) ??
             propertyValueName);
            okConverter.Write(writer, value.OkValue, options);
        }
        else
        {
            writer.WritePropertyName
            (options.PropertyNamingPolicy?.ConvertName(propertyStatusName) ??
             propertyStatusName);
            writer.WriteStringValue(statusError);

            writer.WritePropertyName
            (options.PropertyNamingPolicy?.ConvertName(propertyValueName) ??
             propertyValueName);
            errorConverter.Write(writer, value.ErrorValue, options);
        }

        writer.WriteEndObject();
    }
}
