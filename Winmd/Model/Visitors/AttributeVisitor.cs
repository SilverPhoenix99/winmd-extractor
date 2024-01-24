namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using ClassExtensions;
using Mono.Cecil;

class AttributeVisitor : IVisitor<CustomAttribute, AttributeModel>
{
    public static readonly AttributeVisitor Instance = new();

    private static readonly (string Name, string Namespace) GuidName = typeof(GuidAttribute).GetQualifiedName()!;

    private AttributeVisitor() {}

    public AttributeModel Visit(CustomAttribute attribute)
    {
        var (name, @namespace) = attribute.AttributeType.GetQualifiedName();
        name = name.StripEnd("Attribute");

        if (name == "Guid" && @namespace == "Windows.Win32.Foundation.Metadata")
        {
            // Special case
            return CreateGuid(attribute);
        }

        return new AttributeModel(name)
        {
            Namespace = @namespace,
            Arguments = attribute.ConstructorArguments
                .Select(arg => arg.Accept(AttributeArgumentVisitor.Instance))
                .ToImmutableList(),
            Properties = attribute.Fields.Concat(attribute.Properties)
                .ToImmutableDictionary(
                    a => a.Name,
                    a => a.Argument.Value
                )
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
                TypeModel.StringType,
                guid.ToString()
            ))
        };
    }
}
