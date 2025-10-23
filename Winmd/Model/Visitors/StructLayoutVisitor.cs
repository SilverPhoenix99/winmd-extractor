using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Winmd.ClassExtensions;
using static Mono.Cecil.TypeAttributes;

namespace Winmd.Model.Visitors;

internal class StructLayoutVisitor : IVisitor<TypeDefinition, AnnotationModel?>
{
    public static readonly StructLayoutVisitor Instance = new();

    private static readonly (string Name, string Namespace) StructLayoutName =
        typeof(StructLayoutAttribute).GetQualifiedName()!;

    private StructLayoutVisitor() {}

    [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
    public AnnotationModel? Visit(TypeDefinition type)
    {
        var layout = (type.Attributes & LayoutMask) switch
        {
            AutoLayout => LayoutKind.Auto,
            SequentialLayout => LayoutKind.Sequential,
            ExplicitLayout => LayoutKind.Explicit,
            _ => throw new UnreachableException()
        };

        CharSet? charSet = (type.Attributes & StringFormatMask) switch
        {
            AnsiClass => null, // defaults
            AutoClass => null,
            UnicodeClass => CharSet.Unicode,
            _ => throw new UnreachableException()
        };

        if (
            (layout == LayoutKind.Auto || (type.IsValueType && layout == LayoutKind.Sequential))
            && charSet is null
            && type is { PackingSize: < 1, ClassSize: < 1 }
        )
        {
            return null;
        }

        var properties = new Dictionary<string, object> { { "Layout", layout.ToString() } };

        if (charSet is not null && charSet != CharSet.Ansi)
        {
            properties["CharSet"] = charSet;
        }

        if (type.PackingSize > 0)
        {
            properties["Pack"] = type.PackingSize;
        }

        if (type.ClassSize > 0)
        {
            properties["Size"] = type.ClassSize;
        }

        return new AnnotationModel(StructLayoutName.Name, StructLayoutName.Namespace)
        {
            Properties = properties.ToImmutableDictionary()
        };
    }
}
