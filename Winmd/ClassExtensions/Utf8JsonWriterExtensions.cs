namespace Winmd.ClassExtensions;

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

record JsonProperty(string Name, object? Value, Type Type);

static class Utf8JsonWriterExtensions
{
    public static void WriteObject(
        this Utf8JsonWriter writer,
        JsonSerializerOptions options,
        IEnumerable<JsonProperty> properties
    )
    {
        writer.WriteStartObject();

        properties = options.DefaultIgnoreCondition switch
        {
            JsonIgnoreCondition.Always => ImmutableList<JsonProperty>.Empty,
            JsonIgnoreCondition.WhenWritingNull => properties.Where(p => p.Value is not null),
            JsonIgnoreCondition.WhenWritingDefault => throw new NotSupportedException(),
            _ => properties
        };

        foreach (var property in properties)
        {
            var name = options.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;
            writer.WritePropertyName(name);
            JsonSerializer.Serialize(writer, property.Value, property.Type, options);
        }

        writer.WriteEndObject();
    }
}
