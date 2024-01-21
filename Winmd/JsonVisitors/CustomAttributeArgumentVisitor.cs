namespace Winmd.JsonVisitors;

using System.Text.Json.Nodes;
using Mono.Cecil;

class CustomAttributeArgumentVisitor :
    IVisitor<CustomAttributeArgument, JsonObject>,
    IVisitor<CustomAttributeNamedArgument, JsonObject>
{
    public JsonObject Visit(CustomAttributeArgument arg) => AsJson(null, arg);

    public JsonObject Visit(CustomAttributeNamedArgument arg) => AsJson(arg.Name, arg.Argument);

    private static JsonObject AsJson(string? name, CustomAttributeArgument arg) =>
        AsJson(name, arg.Type.Resolve(), arg.Value);

    public static JsonObject AsJson(string? name, TypeDefinition argumentType, object? value) =>
        new()
        {
            ["Name"] = name,
            ["Type"] = argumentType.FullName,
            ["Value"] = JsonValue.Create(value),
        };
}
