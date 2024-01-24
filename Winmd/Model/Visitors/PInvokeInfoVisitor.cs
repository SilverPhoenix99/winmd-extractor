namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ClassExtensions;
using Mono.Cecil;

class PInvokeInfoVisitor : IVisitor<MethodDefinition, AttributeModel?>
{
    public static readonly PInvokeInfoVisitor Instance = new();

    private static readonly (string Name, string Namespace) DllImport = typeof(DllImportAttribute).GetQualifiedName()!;

    private PInvokeInfoVisitor() {}

    public AttributeModel? Visit(MethodDefinition method)
    {
        if (!method.IsPInvokeImpl)
        {
            return null;
        }

        var info = method.PInvokeInfo;

        var properties = new Dictionary<string, object>();

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

        return new AttributeModel(DllImport.Name)
        {
            Namespace = DllImport.Namespace,
            Arguments = ImmutableList.Create( new AttributeArgumentModel(TypeModel.StringType, info.Module.Name)),
            Properties = properties.ToImmutableDictionary()
        };
    }

    private static CallingConvention? GetCallingConvention(PInvokeAttributes attributes)
    {
        var masked = attributes & PInvokeAttributes.CallConvMask;
        var callingConvention = (CallingConvention) ((ushort) masked >> 8);
        return callingConvention == CallingConvention.Winapi ? null : callingConvention;
    }

    [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
    private static CharSet? GetCharSet(PInvokeAttributes attributes)
    {
        return (attributes & PInvokeAttributes.CharSetMask) switch
        {
            PInvokeAttributes.CharSetAnsi => null, // defaults
            PInvokeAttributes.CharSetAuto => null,
            PInvokeAttributes.CharSetNotSpec => null,
            PInvokeAttributes.CharSetUnicode => CharSet.Unicode,
            _ => throw new UnreachableException()
        };
    }
}
