namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using ClassExtensions;
using Mono.Cecil;
using Architecture = Architecture;

class AnnotationVisitor : IVisitor<CustomAttribute, AnnotationModel>
{
    public static readonly AnnotationVisitor Instance = new();

    private const string Metadata = "Windows.Win32.Foundation.Metadata";
    private static readonly (string Name, string Namespace) SupportedArchitecture = ("SupportedArchitecture", Metadata);
    private static readonly (string Name, string Namespace) Guid = typeof(GuidAttribute).GetQualifiedName()!;

    private AnnotationVisitor() {}

    public AnnotationModel Visit(CustomAttribute attribute) =>
        CreateGuid(attribute)
        ?? CreateArchitecture(attribute)
        ?? CreateDefault(attribute);

    private static AnnotationModel? CreateGuid(ICustomAttribute attribute)
    {
        var qualifiedName = attribute.AttributeType.GetQualifiedName();
        if (!qualifiedName.Equals((Guid.Name, Metadata)))
        {
            return null;
        }

        var args = new List<object>(
            from a in attribute.ConstructorArguments
            select a.Value
        );

        var guid = new Guid(
            (uint) args[0],
            (ushort) args[1],
            (ushort) args[2],
            (byte) args[3],
            (byte) args[4],
            (byte) args[5],
            (byte) args[6],
            (byte) args[7],
            (byte) args[8],
            (byte) args[9],
            (byte) args[10]
        );

        return new AnnotationModel(Guid.Name, Metadata)
        {
            Arguments = ImmutableList.Create(new AnnotationArgumentModel(
                guid.ToString(),
                TypeModel.StringType
            ))
        };
    }

    private static AnnotationModel? CreateArchitecture(ICustomAttribute attribute)
    {
        var qualifiedName = attribute.AttributeType.GetQualifiedName();
        if (!qualifiedName.Equals(SupportedArchitecture))
        {
            return null;
        }

        var value = (Architecture) (int) attribute.ConstructorArguments[0].Value;
        var architectures =
            from arch in FlagsEnumVisitor.Instance.Visit(value)
            select new AnnotationArgumentModel(arch.ToString(), TypeModel.StringType);

        return new AnnotationModel(SupportedArchitecture.Name, Metadata)
        {
            Arguments = architectures.ToImmutableList()
        };
    }

    private static AnnotationModel CreateDefault(ICustomAttribute attribute)
    {
        var (name, @namespace) = attribute.AttributeType.GetQualifiedName();
        return new AnnotationModel(name, @namespace)
        {
            Arguments = attribute.ConstructorArguments
                .Select(arg => arg.Accept(AnnotationArgumentVisitor.Instance))
                .ToImmutableList(),
            Properties = attribute.Fields.Concat(attribute.Properties)
                .ToImmutableDictionary(
                    a => a.Name,
                    a => a.Argument.Value
                )
        };
    }
}
