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

        var args = new List<AttributeArgumentModel>
        {
            new(TypeModel.StringType, info.Module.Name)
        };

        if (info.EntryPoint != method.Name)
        {
            args.Add(new AttributeArgumentModel("EntryPoint", info.EntryPoint));
        }

        var charSet = GetCharSet(info.Attributes);
        if (charSet is not null)
        {
            args.Add(new AttributeArgumentModel("CharSet", charSet));
        }

        if (info.IsNoMangle)
        {
            args.Add(new AttributeArgumentModel("ExactSpelling", true));
        }

        if (info.SupportsLastError)
        {
            args.Add(new AttributeArgumentModel("SetLastError", true));
        }

        if (!method.ImplAttributes.HasFlag(MethodImplAttributes.PreserveSig))
        {
            args.Add(new AttributeArgumentModel("PreserveSig", false));
        }

        var callingConvention = GetCallingConvention(info.Attributes);
        if (callingConvention is not null)
        {
            args.Add(new AttributeArgumentModel("CallingConvention", callingConvention));
        }

        if (info.IsBestFitDisabled)
        {
            args.Add(new AttributeArgumentModel("BestFitMapping", false));
        }

        if (info.IsThrowOnUnmappableCharEnabled)
        {
            args.Add(new AttributeArgumentModel("ThrowOnUnmappableChar", true));
        }

        return new AttributeModel(DllImport.Name)
        {
            Namespace = DllImport.Namespace,
            Arguments = args.ToImmutableList()
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
