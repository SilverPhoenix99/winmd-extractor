namespace Winmd;

using System.Text.Json.Nodes;
using JsonVisitors;
using Mono.Cecil;

class JsonGenerator : IVisitor<TypeDefinition, JsonObject>
{
    // ReSharper disable once InconsistentNaming
    private static readonly TypeAttributesVisitor typeAttributesVisitor = new();

    // ReSharper disable once InconsistentNaming
    private static readonly CustomAttributeVisitor customAttributeVisitor = new();

    // ReSharper disable once InconsistentNaming
    private static readonly EnumVisitor enumVisitor = new();

    public JsonObject Visit(TypeDefinition type)
    {
        var json = new JsonObject
        {
            ["BaseType"] = type.IsInterface ? "interface" : type.BaseType?.FullName,
            ["Interfaces"] = VisitInterfaces(type),
            ["Attributes"] = type.Attributes.Accept(typeAttributesVisitor),
            ["CustomAttributes"] = Visit(type.CustomAttributes),
        };

        if (type.ClassSize > 0)
        {
            json["ClassSize"] = type.ClassSize;
        }

        if (type.PackingSize > 0)
        {
            json["PackingSize"] = type.PackingSize;
        }

        /*
        Types that can show up:
            * System.Enum
            * System.MulticastDelegate - callbacks
            * System.Object - class Apis, the list of available functions
            * System.ValueType - structs, unions, or typedefs (when it has a single field)
        */

        if (type.IsEnum)
        {
            json["Enum"] = type.Accept(enumVisitor);
        }
        else if (type.IsDelegate())
        {
            // TODO json["Delegate"] = type.Accept(delegateVisitor);
        }
        else if (type.IsValueType)
        {
            // TODO json["Struct"] = type.Accept(structVisitor);
        }
        else
        {
            // TODO json["Class"] = type.Accept(classVisitor);
        }

        return json;
    }

    internal static JsonArray CreateArray<T>(IEnumerable<T> source) where T : JsonNode?
    {
        return new JsonArray(
            (
                from s in source
                where s is not null
                select (JsonNode) s
            ).ToArray()
        );
    }

    private static JsonArray VisitInterfaces(TypeDefinition type)
    {
        var interfaceTypeVisitor = new InterfaceTypeVisitor(type);

        return CreateArray(
            from i in type.Interfaces
            let v = i.Accept(interfaceTypeVisitor)
            where v is not null
            select v!
        );
    }

    private static JsonArray Visit(IEnumerable<CustomAttribute> attributes)
    {
        var customAttributes = from a in attributes
            select a.Accept(customAttributeVisitor);

        return CreateArray(customAttributes);
    }
}
