namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ClassExtensions;
using Mono.Cecil;
using static Mono.Cecil.PInvokeAttributes;

class PInvokeInfoVisitor : IVisitor<MethodDefinition, AnnotationModel?>
{
    public static readonly PInvokeInfoVisitor Instance = new();

    private static readonly (string Name, string Namespace) DllImport = typeof(DllImportAttribute).GetQualifiedName()!;

    private PInvokeInfoVisitor() {}

    public AnnotationModel? Visit(MethodDefinition method)
    {
        if (!method.IsPInvokeImpl)
        {
            return null;
        }

        var info = method.PInvokeInfo;

        var properties = new Dictionary<string, object> { { "Library", info.Module.Name } };

        if (info.EntryPoint != method.Name)
        {
            properties["EntryPoint"] = info.EntryPoint;
        }

        var charSet = GetCharSet(info.Attributes);
        if (charSet is not null)
        {
            properties["CharSet"] = charSet;
        }

        if (info.IsNoMangle)
        {
            properties["ExactSpelling"] = true;
        }

        if (info.SupportsLastError)
        {
            properties["SetLastError"] = true;
        }

        if (!method.ImplAttributes.HasFlag(MethodImplAttributes.PreserveSig))
        {
            properties["PreserveSig"] = false;
        }

        var callingConvention = GetCallingConvention(info.Attributes);
        if (callingConvention is not null)
        {
            properties["CallingConvention"] = callingConvention;
        }

        if (info.IsBestFitDisabled)
        {
            properties["BestFitMapping"] = false;
        }

        if (info.IsThrowOnUnmappableCharEnabled)
        {
            properties["ThrowOnUnmappableChar"] = true;
        }

        return new AnnotationModel(DllImport.Name, DllImport.Namespace)
        {
            Properties = properties.ToImmutableDictionary()
        };
    }

    private static CallingConvention? GetCallingConvention(PInvokeAttributes attributes)
    {
        var masked = attributes & CallConvMask;
        var callingConvention = (CallingConvention) ((ushort) masked >> 8);
        return callingConvention == CallingConvention.Winapi ? null : callingConvention;
    }

    [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
    private static CharSet? GetCharSet(PInvokeAttributes attributes)
    {
        return (attributes & CharSetMask) switch
        {
            CharSetAnsi => null, // defaults
            CharSetAuto => null,
            CharSetNotSpec => null,
            CharSetUnicode => CharSet.Unicode,
            _ => throw new UnreachableException()
        };
    }
}
