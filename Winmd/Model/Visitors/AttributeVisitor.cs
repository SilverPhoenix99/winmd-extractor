namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class AttributeVisitor : IVisitor<CustomAttribute, AttributeModel>
{
    public static readonly AttributeVisitor Instance = new();

    private AttributeVisitor() {}

    public AttributeModel Visit(CustomAttribute value)
    {
        var (name, ns) = value.AttributeType.GetQualifiedName();
        if (name.EndsWith("Attribute"))
        {
            name = name[..^9];
        }

        var ctorArgs =
            from arg in value.ConstructorArguments
            select arg.Accept<AttributeArgumentModel>(AttributeArgumentVisitor.Instance);

        var namedArgs =
            from arg in value.Fields.Concat(value.Properties)
            select arg.Accept<AttributeArgumentModel>(AttributeArgumentVisitor.Instance);

        return new AttributeModel(name)
        {
            Namespace = ns,
            Arguments = ctorArgs.Concat(namedArgs).ToImmutableList()
        };
    }
}
