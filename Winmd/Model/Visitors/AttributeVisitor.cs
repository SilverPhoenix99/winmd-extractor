namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using ClassExtensions;
using Mono.Cecil;
using Architecture = Architecture;

class AttributeVisitor : IVisitor<CustomAttribute, AttributeModel>
{
    public static readonly AttributeVisitor Instance = new();

    private const string Metadata = "Windows.Win32.Foundation.Metadata";
    private static readonly (string Name, string Namespace) SupportedArchitecture = ("SupportedArchitecture", Metadata);
    private static readonly (string Name, string Namespace) Guid = typeof(GuidAttribute).GetQualifiedName()!;

    private AttributeVisitor() {}

    public AttributeModel Visit(CustomAttribute attribute) =>
        CreateGuid(attribute)
        ?? CreateArchitecture(attribute)
        ?? CreateDefault(attribute);

    private static AttributeModel? CreateGuid(ICustomAttribute attribute)
    {
        var qualifiedName = attribute.AttributeType.GetQualifiedName();
        if (!qualifiedName.Equals((Guid.Name, Metadata)))
        {
            return null;
        }

        var args = attribute.ConstructorArguments
            .Select(a => a.Value)
            .ToArray();

        var guid = (Guid) Activator.CreateInstance(typeof(Guid), args)!;

        return new AttributeModel(Guid.Name)
        {
            Namespace = Metadata,
            Arguments = ImmutableList.Create(new AttributeArgumentModel(
                TypeModel.StringType,
                guid.ToString()
            ))
        };
    }

    private static AttributeModel? CreateArchitecture(ICustomAttribute attribute)
    {
        var qualifiedName = attribute.AttributeType.GetQualifiedName();
        if (!qualifiedName.Equals(SupportedArchitecture))
        {
            return null;
        }

        var value = (Architecture) (int) attribute.ConstructorArguments[0].Value;
        var architectures = FlagsEnumVisitor.Instance
            .Visit(value)
            .Select(arch => new AttributeArgumentModel(TypeModel.StringType, arch.ToString()))
            .ToImmutableList();

        return new AttributeModel(SupportedArchitecture.Name)
        {
            Namespace = Metadata,
            Arguments = architectures
        };
    }

    private static AttributeModel CreateDefault(ICustomAttribute attribute)
    {
        var (name, @namespace) = attribute.AttributeType.GetQualifiedName();
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
}
