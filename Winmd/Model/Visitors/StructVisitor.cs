namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ClassExtensions;
using Mono.Cecil;

class StructVisitor : BaseObjectVisitor<StructModel>
{
    public static readonly StructVisitor Instance = new();

    protected StructVisitor() {}

    public override StructModel Visit(TypeDefinition type) =>
        new(type.Name, GetAnnotations(type))
        {
            Nesting = GetNesting(type),
            Fields = GetFields(type)
        };

    protected static IImmutableList<string>? GetNesting(TypeDefinition type) =>
        type.GetNesting()?.Select(t => t.Name).ToImmutableList();

    protected static ImmutableList<FieldModel> GetFields(TypeDefinition type) =>
        ImmutableList.CreateRange(
            from field in type.Fields
            where field.IsPublic && !field.IsStatic && !field.IsSpecialName
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

class UnionVisitor : StructVisitor
{
    public new static readonly UnionVisitor Instance = new();

    private UnionVisitor() {}

    public override UnionModel Visit(TypeDefinition type) =>
        new(type.Name, GetAnnotations(type))
        {
            Nesting = GetNesting(type),
            Fields = GetFields(type)
        };

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
