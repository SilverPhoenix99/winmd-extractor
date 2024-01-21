namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;
using Mono.Cecil;
using Mono.Cecil.Rocks;

class EnumVisitor : IVisitor<TypeDefinition, JsonObject>
{
    public static readonly EnumVisitor Instance = new();

    public JsonObject Visit(TypeDefinition type)
    {
        var json = new JsonObject();

        // Enums are always a subtype of primitives.
        // The namespace will always be System,
        // so there is no point in using the full name.
        var baseType = type.GetEnumUnderlyingType().Name;
        if (baseType != nameof(Int32))
        {
            json["Type"] = baseType;
        }

        json["Elements"] = JsonGenerator.CreateArray(
            from field in type.Fields
            where !field.IsSpecialName
            select field.Accept(EnumMemberVisitor.Instance)
        );

        return json;
    }
}
