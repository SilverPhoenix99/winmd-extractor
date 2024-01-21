namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;
using Mono.Cecil;

class CustomAttributeVisitor : IVisitor<CustomAttribute, JsonObject>
{
    private static readonly CustomAttributeArgumentVisitor ArgumentVisitor = new();

    public JsonObject Visit(CustomAttribute attribute)
    {
        var args = attribute.ConstructorArguments
            .Select(arg => arg.Accept<JsonObject>(ArgumentVisitor))
            .Concat(
                attribute.Fields
                    .Concat(attribute.Properties)
                    .Select(arg => arg.Accept<JsonObject>(ArgumentVisitor))
            );

        return new JsonObject
        {
            ["Name"] = JsonValue.Create(attribute.AttributeType.FullName),
            ["Arguments"] = JsonGenerator.CreateArray(args)
        };
    }
}
