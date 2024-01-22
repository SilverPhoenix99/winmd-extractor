namespace Winmd.Model.Visitors;

using ClassExtensions;
using Mono.Cecil;

class CustomAttributeArgumentVisitor :
    IVisitor<CustomAttributeArgument, AttributeArgumentModel>,
    IVisitor<CustomAttributeNamedArgument, AttributeArgumentModel>
{
    public static readonly CustomAttributeArgumentVisitor Instance = new();

    private CustomAttributeArgumentVisitor() {}

    public AttributeArgumentModel Visit(CustomAttributeArgument value) =>
        ToModel(null, value.Type.Resolve(), value.Value);

    public AttributeArgumentModel Visit(CustomAttributeNamedArgument value) =>
        ToModel(
            value.Name,
            value.Argument.Type,
            value.Argument.Value
        );

    private static AttributeArgumentModel ToModel(string? name, TypeReference argumentType, object? value)
    {
        var parseValue = ParseValue(argumentType.Resolve(), value);
        return name != null
            ? new AttributeArgumentModel(name, parseValue)
            : new AttributeArgumentModel(argumentType.Accept(TypeVisitor.Instance), parseValue);
    }

    private static object? ParseValue(TypeDefinition argumentType, object? value) =>
        !argumentType.IsEnum || value is null
            ? value
            : argumentType.Fields.FirstOrDefault(f => Equals(f.Constant, value))?.Name ?? value;
}
