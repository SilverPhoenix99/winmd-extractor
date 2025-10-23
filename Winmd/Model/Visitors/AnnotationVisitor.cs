using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Winmd.ClassExtensions;

namespace Winmd.Model.Visitors;

using Architecture = Architecture;

internal class AnnotationVisitor : IVisitor<CustomAttribute, AnnotationModel>
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

        object[] args =
        [..
            from a in attribute.ConstructorArguments
            select a.Value
        ];

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
            Properties = ImmutableDictionary<string, object>.Empty.Add("Value", guid.ToString())
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
        string[] archs =
        [
            ..
            from arch in FlagsEnumVisitor.Instance.Visit(value)
            select arch.ToString()
        ];

        return new AnnotationModel(SupportedArchitecture.Name, Metadata)
        {
            Properties = ImmutableDictionary<string, object>.Empty.Add("Value", archs)
        };
    }

    private static AnnotationModel CreateDefault(CustomAttribute attribute)
    {
        var (name, @namespace) = attribute.AttributeType.GetQualifiedName();

        var ctorArgs = GetConstructorArguments(attribute);

        var properties = attribute.Fields.Concat(attribute.Properties)
            .Select(a => KeyValuePair.Create(a.Name, a.Argument.Value));

        return new AnnotationModel(name, @namespace)
        {
            Properties = ctorArgs.Concat(properties).ToImmutableDictionary()
        };
    }

    private static IEnumerable<KeyValuePair<string, object>> GetConstructorArguments(CustomAttribute attribute)
    {
        if (!attribute.HasConstructorArguments)
        {
            yield break;
        }

        var type = attribute.AttributeType.Resolve();
        var ctor = attribute.Constructor.Resolve();
        if (ctor is null)
        {
            var argCount = attribute.ConstructorArguments.Count;
            ctor = type.GetConstructors()
                .Single(c => c.Parameters.Count == argCount)
                .Resolve();
        }

        var typeProperties = new HashSet<string>(
            type.Properties.Select(p => p.Name),
            StringComparer.OrdinalIgnoreCase
        );

        var args = ctor.Parameters.Zip(attribute.ConstructorArguments);

        foreach (var arg in args)
        {
            yield return KeyValuePair.Create(GetName(arg.First.Name), arg.Second.Value);
        }

        yield break;

        string GetName(string n) => typeProperties.TryGetValue(n, out var p) ? p : n;
    }
}
