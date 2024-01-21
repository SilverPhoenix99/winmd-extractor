namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;
using Mono.Cecil;

class CustomAttributeVisitor : IVisitor<CustomAttribute, JsonObject>
{
    public static readonly CustomAttributeVisitor Instance = new();

    public JsonObject Visit(CustomAttribute attribute)
    {
        var args = attribute.ConstructorArguments
            .Select(arg => arg.Accept<JsonObject>(CustomAttributeArgumentVisitor.Instance))
            .Concat(
                attribute.Fields
                    .Concat(attribute.Properties)
                    .Select(arg => arg.Accept<JsonObject>(CustomAttributeArgumentVisitor.Instance))
            );

        return new JsonObject
        {
            ["Name"] = JsonValue.Create(attribute.AttributeType.FullName),
            ["Arguments"] = JsonGenerator.CreateArray(args)
        };
    }
}
