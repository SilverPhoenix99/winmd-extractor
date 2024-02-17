namespace Winmd.Model.Visitors;

using ClassExtensions;
using Mono.Cecil;

class AnnotationArgumentVisitor : IVisitor<CustomAttributeArgument, AnnotationArgumentModel>
{
    public static readonly AnnotationArgumentVisitor Instance = new();

    private AnnotationArgumentVisitor() {}

    public AnnotationArgumentModel Visit(CustomAttributeArgument argument)
    {
        var parseValue = ParseValue(argument.Type.Resolve(), argument.Value);
        return new AnnotationArgumentModel(parseValue, argument.Type.Accept(TypeVisitor.Instance));
    }

    private static object? ParseValue(TypeDefinition argumentType, object? value) =>
        !argumentType.IsEnum || value is null
            ? value
            : argumentType.Fields.FirstOrDefault(f => Equals(f.Constant, value))?.Name ?? value;
}
