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

        var name = attribute.AttributeType.Name;
        if (name.EndsWith("Attribute"))
        {
            name = name[..^9];
        }

        return new JsonObject
        {
            ["Name"] = name,
            ["Namespace"] = attribute.AttributeType.Namespace,
            ["Arguments"] = JsonGenerator.CreateArray(args)
        };
    }
}
