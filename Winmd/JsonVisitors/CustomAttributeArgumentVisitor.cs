namespace Winmd.JsonVisitors;

using System.Reflection;
using System.Text.Json.Nodes;

class CustomAttributeArgumentVisitor :
    IVisitor<CustomAttributeTypedArgument, JsonObject>,
    IVisitor<CustomAttributeNamedArgument, JsonObject>
{
    public JsonObject Visit(CustomAttributeTypedArgument arg) => Create(null, false, arg);

    public JsonObject Visit(CustomAttributeNamedArgument arg) =>
        Create(arg.MemberName, arg.IsField, arg.TypedValue);

    private static JsonObject Create(string? name, bool isField, CustomAttributeTypedArgument arg) =>
        Create(name, isField, arg.ArgumentType, arg.Value);

    public static JsonObject Create(string? name, bool isField, Type argumentType, object? value)
    {
        if (argumentType.IsEnum && value is not null)
        {
            value = argumentType.GetEnumName(value) ?? value;
        }

        return new JsonObject
        {
            ["Name"] = name,
            ["IsField"] = isField,
            ["Type"] = argumentType.FullName,
            ["Value"] = JsonValue.Create(value),
        };
    }
}