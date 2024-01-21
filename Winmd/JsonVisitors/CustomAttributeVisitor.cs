namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;
using ClassExtensions;
using Mono.Cecil;

class CustomAttributeVisitor : IVisitor<CustomAttribute, JsonObject>
{
    public static readonly CustomAttributeVisitor Instance = new();

    public JsonObject Visit(CustomAttribute attribute)
    {
        var (name, ns) = attribute.AttributeType.GetQualifiedName();
        if (name.EndsWith("Attribute"))
        {
            name = name[..^9];
        }

        var args = attribute.ConstructorArguments
            .Select(arg => arg.Accept<JsonObject>(CustomAttributeArgumentVisitor.Instance))
            .Concat(
                attribute.Fields
                    .Concat(attribute.Properties)
                    .Select(arg => arg.Accept<JsonObject>(CustomAttributeArgumentVisitor.Instance))
            );

        return new JsonObject
        {
            ["Name"] = name,
            ["Namespace"] = ns,
            ["Arguments"] = JsonGenerator.CreateArray(args)
        };
    }
}
