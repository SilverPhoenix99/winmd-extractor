namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using ClassExtensions;
using Mono.Cecil;

class AttributeVisitor : IVisitor<CustomAttribute, AttributeModel>
{
    public static readonly AttributeVisitor Instance = new();

    private static readonly (string Name, string Namespace) GuidName = typeof(GuidAttribute).GetQualifiedName()!;

    private static readonly TypeModel StringType = new("String");

    private AttributeVisitor() {}

    public AttributeModel Visit(CustomAttribute attribute)
    {
        var (name, @namespace) = attribute.AttributeType.GetQualifiedName();
        if (name.EndsWith("Attribute"))
        {
            name = name[..^9];
        }

        if (name == "Guid" && @namespace == "Windows.Win32.Foundation.Metadata")
        {
            // Special case
            return CreateGuid(attribute);
        }

        var ctorArgs =
            from arg in attribute.ConstructorArguments
            select arg.Accept<AttributeArgumentModel>(AttributeArgumentVisitor.Instance);

        var namedArgs =
            from arg in attribute.Fields.Concat(attribute.Properties)
            select arg.Accept<AttributeArgumentModel>(AttributeArgumentVisitor.Instance);

        return new AttributeModel(name)
        {
            Namespace = @namespace,
            Arguments = ctorArgs.Concat(namedArgs).ToImmutableList()
        };
    }

    private static AttributeModel CreateGuid(ICustomAttribute attribute)
    {
        var args = attribute.ConstructorArguments
            .Select(a => a.Value)
            .ToArray();

        var guid = (Guid) Activator.CreateInstance(typeof(Guid), args)!;

        return new AttributeModel(GuidName.Name)
        {
            Namespace = GuidName.Namespace,
            Arguments = ImmutableList.Create(new AttributeArgumentModel(
                StringType,
                guid.ToString()
            ))
        };
    }
}
