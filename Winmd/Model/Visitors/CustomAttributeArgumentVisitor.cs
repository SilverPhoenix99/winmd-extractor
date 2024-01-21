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
            value.Argument.Type.Resolve(),
            value.Argument.Value
        );

    private static CustomAttributeArgumentModel ToModel(string? name, TypeDefinition argumentType, object? value) =>
        new(
            name: name,
            value: ParseValue(argumentType, value),
            type: argumentType.Accept(TypeModelVisitor.Instance)
        );

    private static object? ParseValue(TypeDefinition argumentType, object? value) =>
        !argumentType.IsEnum || value is null
            ? value
            : argumentType.Fields.FirstOrDefault(f => Equals(f.Constant, value))?.Name ?? value;
}
