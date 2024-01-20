namespace Winmd.JsonVisitors;

using System.Reflection;
using System.Text.Json.Nodes;

class CustomAttributeVisitor : IVisitor<CustomAttributeData, JsonObject>
{
    private static readonly CustomAttributeArgumentVisitor ArgumentVisitor = new();

    public JsonObject Visit(CustomAttributeData attribute)
    {
        var args =
            from a in attribute.ConstructorArguments
            select a.Accept<JsonObject>(ArgumentVisitor);

        var namedArgs =
            from a in attribute.NamedArguments
            select a.Accept<JsonObject>(ArgumentVisitor);

        return new JsonObject
        {
            ["Name"] = JsonValue.Create(attribute.AttributeType.FullName),
            ["Arguments"] = JsonGenerator.CreateArray(args.Concat(namedArgs))
        };
    }
}
