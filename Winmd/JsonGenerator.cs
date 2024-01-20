namespace Winmd;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using JsonVisitors;

class JsonGenerator : IVisitor<Type, JsonObject>
{
    // ReSharper disable once InconsistentNaming
    private static readonly TypeAttributesVisitor typeAttributesVisitor = new();

    // ReSharper disable once InconsistentNaming
    private static readonly CustomAttributeVisitor customAttributeVisitor = new();

    // ReSharper disable once InconsistentNaming
    private static readonly StructLayoutAttributeVisitor structLayoutAttributeVisitor = new();

    // ReSharper disable once InconsistentNaming
    private static readonly EnumVisitor enumVisitor = new();

    public JsonObject Visit(Type type)
    {
        var json = new JsonObject
        {
            ["BaseType"] = type.IsInterface ? "interface" : type.BaseType?.FullName,
            ["Interfaces"] = VisitInterfaces(type),
            ["Attributes"] = type.Attributes.Accept(typeAttributesVisitor),
            ["CustomAttributes"] = Visit(type.StructLayoutAttribute, type.GetCustomAttributesData()),
        };

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
        else if (type.IsAssignableTo(typeof(Delegate)))
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
                select (JsonNode)s
            ).ToArray()
        );
    }

    private static JsonArray VisitInterfaces(Type type)
    {
        var interfaceTypeVisitor = new InterfaceTypeVisitor(type);

        return CreateArray(
            from i in type.GetInterfaces()
            let v = i.Accept(interfaceTypeVisitor)
            where v is not null
            select v!
        );
    }

    private static JsonArray Visit(
        StructLayoutAttribute? structLayoutAttribute,
        IEnumerable<CustomAttributeData> attributes
    )
    {
        var customAttributes = from a in attributes
            where a.GetType().FullName != "System.Reflection.TypeLoading.Ecma.EcmaCustomAttributeData"
            select a.Accept(customAttributeVisitor);

        var structLayout = structLayoutAttribute?.Accept(structLayoutAttributeVisitor);

        if (structLayout is not null)
        {
            customAttributes = customAttributes.Prepend(structLayout);
        }

        return CreateArray(customAttributes);
    }
}
