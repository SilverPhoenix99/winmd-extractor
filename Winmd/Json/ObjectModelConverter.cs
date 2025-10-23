using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Winmd.ClassExtensions;
using Winmd.Model;
using JsonProperty = Winmd.ClassExtensions.JsonProperty;

namespace Winmd.Json;

internal class ObjectModelConverter : JsonConverter<BaseObjectModel>
{
    public override void Write(Utf8JsonWriter writer, BaseObjectModel model, JsonSerializerOptions options)
    {
        var type = model.GetType();

        var typeOrder = GetTypeChain(type)
            .Reverse()
            .Select((t, i) => new { Type = t, Order = i + 1 })
            .ToImmutableDictionary(
                t => t.Type,
                t => t.Order
            );

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(p => typeOrder.GetValueOrDefault(GetOrigin(p)))
            .Select(p => new JsonProperty
            (
                p.Name,
                p.GetValue(model),
                p.PropertyType
            ));

        writer.WriteObject(options, properties);
    }

    public override BaseObjectModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    private static IEnumerable<Type> GetTypeChain(Type type)
    {
        yield return type;

        for (
            var baseType = type.BaseType;
            baseType is not null && baseType != typeof(object);
            baseType = baseType.BaseType
        )
        {
            yield return baseType;
        }
    }

    private static Type GetOrigin(MemberInfo property)
    {
        var type = property.DeclaringType!;
        var baseType = type.BaseType;

        while (baseType is not null && baseType != typeof(object))
        {
            if (baseType.GetProperty(property.Name) is not null)
            {
                type = baseType;
            }

            baseType = baseType.BaseType;
        }

        return type;
    }
}
