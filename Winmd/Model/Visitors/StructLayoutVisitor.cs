namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ClassExtensions;
using Mono.Cecil;

class StructLayoutVisitor : IVisitor<TypeDefinition, AttributeModel?>
{
    public static readonly StructLayoutVisitor Instance = new();

    private static readonly (string Name, string Namespace) StructLayoutName =
        typeof(StructLayoutAttribute).GetQualifiedName()!;

    private StructLayoutVisitor() {}

    [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
    public AttributeModel? Visit(TypeDefinition type)
    {
        var layout = (type.Attributes & TypeAttributes.LayoutMask) switch
        {
            TypeAttributes.AutoLayout => LayoutKind.Auto,
            TypeAttributes.SequentialLayout => LayoutKind.Sequential,
            TypeAttributes.ExplicitLayout => LayoutKind.Explicit,
            _ => throw new UnreachableException()
        };

        CharSet? charSet = (type.Attributes & TypeAttributes.StringFormatMask) switch
        {
            TypeAttributes.AnsiClass => null, // defaults
            TypeAttributes.AutoClass => null,
            TypeAttributes.UnicodeClass => CharSet.Unicode,
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

        var args = new List<AttributeArgumentModel>
        {
            new(TypeModel.LayoutKindType, layout.ToString())
        };

        if (charSet != CharSet.Ansi)
        {
            args.Add(new AttributeArgumentModel("CharSet", charSet.ToString()));
        }

        if (type.PackingSize > 0)
        {
            args.Add(new AttributeArgumentModel("Pack", type.PackingSize));
        }

        if (type.ClassSize > 0)
        {
            args.Add(new AttributeArgumentModel("Size", type.ClassSize));
        }

        return new AttributeModel(StructLayoutName.Name)
        {
            Namespace = StructLayoutName.Namespace,
            Arguments = args.ToImmutableList()
        };
    }
}
