namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class CustomAttributeVisitor : IVisitor<CustomAttribute, AttributeModel>
{
    public static readonly CustomAttributeVisitor Instance = new();

    private CustomAttributeVisitor() {}

    public AttributeModel Visit(CustomAttribute value)
    {
        var (name, ns) = value.AttributeType.GetQualifiedName();
        if (name.EndsWith("Attribute"))
        {
            name = name[..^9];
        }

        var ctorArgs =
            from arg in value.ConstructorArguments
            select arg.Accept<AttributeArgumentModel>(CustomAttributeArgumentVisitor.Instance);

        var namedArgs =
            from arg in value.Fields.Concat(value.Properties)
            select arg.Accept<AttributeArgumentModel>(CustomAttributeArgumentVisitor.Instance);

        return new AttributeModel(name)
        {
            Namespace = ns,
            Arguments = ctorArgs.Concat(namedArgs).ToImmutableList()
        };
    }
}
