namespace Winmd;

using System.Text.Json.Nodes;
using ClassExtensions;
using JsonVisitors;
using Mono.Cecil;

class JsonGenerator : IVisitor<TypeDefinition, JsonObject>
{
    public static readonly JsonGenerator Instance = new();

    public JsonObject Visit(TypeDefinition type)
    {
        var (name, ns) = type.IsInterface
            ? ("interface", null)
            : type.BaseType.GetQualifiedName();

        var json = new JsonObject
        {
            ["BaseType"] = new JsonObject
            {
                ["Name"] = name,
                ["Namespace"] = ns,
            },
            ["Interfaces"] = VisitInterfaces(type),
            ["Attributes"] = type.Attributes.Accept(TypeAttributesVisitor.Instance),
            ["CustomAttributes"] = Visit(type.CustomAttributes),
            ["ClassSize"] = type.ClassSize > 0 ? type.ClassSize : null,
            ["PackingSize"] = type.PackingSize > 0 ? type.PackingSize : null,
        };

        /*
        Types that can show up:
            * System.Enum
            * System.MulticastDelegate - callbacks
            * System.Object - class Apis, the list of available functions
            * System.ValueType - structs, unions, or typedefs (when it has NativeTypedefAttribute)
        */

        if (type.IsEnum)
        {
            json["Enum"] = type.Accept(EnumVisitor.Instance);
        }
        else if (type.IsDelegate())
        {
            json["Delegate"] = type.Accept(DelegateVisitor.Instance);
        }
        else if (type.IsValueType)
        {
            // TODO: json["Typedef"] = when NativeTypedefAttribute
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
        return CreateArray(
            from a in attributes
            select a.Accept(CustomAttributeVisitor.Instance)
        );
    }
}
