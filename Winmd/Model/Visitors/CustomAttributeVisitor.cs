namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class CustomAttributeVisitor : IVisitor<CustomAttribute, CustomAttributeModel>
{
    public static readonly CustomAttributeVisitor Instance = new();

    private CustomAttributeVisitor() {}

    public CustomAttributeModel Visit(CustomAttribute value)
    {
        var (name, ns) = value.AttributeType.GetQualifiedName();
        if (name.EndsWith("Attribute"))
        {
            name = name[..^9];
        }

        var ctorArgs =
            from arg in value.ConstructorArguments
            select arg.Accept<CustomAttributeArgumentModel>(CustomAttributeArgumentVisitor.Instance);

        var namedArgs =
            from arg in value.Fields.Concat(value.Properties)
            select arg.Accept<CustomAttributeArgumentModel>(CustomAttributeArgumentVisitor.Instance);

        return new CustomAttributeModel(name)
        {
            Namespace = ns,
            Arguments = ctorArgs.Concat(namedArgs).ToImmutableList()
        };
    }
}
