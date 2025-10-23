using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;
using Winmd.ClassExtensions;

namespace Winmd.Model.Visitors;

internal class StructVisitor : BaseObjectVisitor<StructModel?>
{
    public static readonly StructVisitor Instance = new();

    protected StructVisitor() {}

    public override StructModel? Visit(TypeDefinition type)
    {
        if (IsCom(type))
        {
            return null;
        }
        
        return new StructModel(type.Name, GetAnnotations(type))
        {
            Nesting = GetNesting(type),
            Fields = GetFields(type)
        };
    }

    protected static bool IsCom(TypeDefinition type)
    {
        var isCom = type.Fields
            .Select(field => field.FieldType)
            .Any(TypeVisitor.IsCom);
        if (isCom)
        {
            return true;
        }

        var nesting = type.GetNesting();
        if (nesting == null)
        {
            return false;
        }

        return nesting
            .Select(t => t.Resolve())
            .SelectMany(t => t.Fields)
            .Select(f => f.FieldType)
            .Any(TypeVisitor.IsCom);
    }

    protected static IImmutableList<string>? GetNesting(TypeDefinition type) =>
        type.GetNesting()?.Select(t => t.Name).ToImmutableList();

    protected static ImmutableList<FieldModel> GetFields(TypeDefinition type) =>
        ImmutableList.CreateRange(
            from field in type.Fields
            where field is { IsPublic: true, IsStatic: false, IsSpecialName: false }
            select new FieldModel(
                field.Name,
                field.FieldType.Accept(TypeVisitor.Instance),
                GetAnnotations(field)
            )
        );

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    private static ImmutableList<AnnotationModel>? GetAnnotations(FieldDefinition field)
    {
        var annotations = ImmutableList.CreateRange(
            from a in field.CustomAttributes
            select a.Accept(AnnotationVisitor.Instance)
        );

        return annotations.IsEmpty ? null : annotations;
    }
}

internal class UnionVisitor : StructVisitor
{
    public new static readonly UnionVisitor Instance = new();

    private UnionVisitor() {}

    public override UnionModel? Visit(TypeDefinition type)
    {
        if (IsCom(type))
        {
            return null;
        }
        
        return new UnionModel(type.Name, GetAnnotations(type))
        {
            Nesting = GetNesting(type),
            Fields = GetFields(type)
        };
    }

    protected override IImmutableList<AnnotationModel>? GetAnnotations(TypeDefinition type)
    {
        var annotations = base.GetAnnotations(type);
        return annotations == null ? null
            : ImmutableList.CreateRange(
                from a in base.GetAnnotations(type)
                where !IsStructLayout(a)
                select a
            );
    }

    private static bool IsStructLayout(AnnotationModel annotation) =>
        annotation is { Name: "StructLayout", Namespace: "System.Runtime.InteropServices" };
}
