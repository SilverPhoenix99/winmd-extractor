namespace Winmd.Model.Visitors;

using ClassExtensions;
using Mono.Cecil;

class AttributeArgumentVisitor : IVisitor<CustomAttributeArgument, AttributeArgumentModel>
{
    public static readonly AttributeArgumentVisitor Instance = new();

    private AttributeArgumentVisitor() {}

    public AttributeArgumentModel Visit(CustomAttributeArgument argument)
    {
        var parseValue = ParseValue(argument.Type.Resolve(), argument.Value);
        return new AttributeArgumentModel(argument.Type.Accept(TypeVisitor.Instance), parseValue);
    }

    private static object? ParseValue(TypeDefinition argumentType, object? value) =>
        !argumentType.IsEnum || value is null
            ? value
            : argumentType.Fields.FirstOrDefault(f => Equals(f.Constant, value))?.Name ?? value;
}
