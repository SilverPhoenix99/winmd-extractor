namespace Winmd.Model.Visitors;

using ClassExtensions;
using Mono.Cecil;

class CustomAttributeArgumentVisitor :
    IVisitor<CustomAttributeArgument, CustomAttributeArgumentModel>,
    IVisitor<CustomAttributeNamedArgument, CustomAttributeArgumentModel>
{
    public static readonly CustomAttributeArgumentVisitor Instance = new();

    private CustomAttributeArgumentVisitor() {}

    public CustomAttributeArgumentModel Visit(CustomAttributeArgument value) =>
        ToModel(null, value.Type.Resolve(), value.Value);

    public CustomAttributeArgumentModel Visit(CustomAttributeNamedArgument value) =>
        ToModel(
            value.Name,
            value.Argument.Type,
            value.Argument.Value
        );

    private static CustomAttributeArgumentModel ToModel(string? name, TypeReference argumentType, object? value) =>
        new(
            name: name,
            value: ParseValue(argumentType.Resolve(), value),
            type: argumentType.Accept(TypeVisitor.Instance)
        );

    private static object? ParseValue(TypeDefinition argumentType, object? value) =>
        !argumentType.IsEnum || value is null
            ? value
            : argumentType.Fields.FirstOrDefault(f => Equals(f.Constant, value))?.Name ?? value;
}
