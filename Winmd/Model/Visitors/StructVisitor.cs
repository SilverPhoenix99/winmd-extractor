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
        new(type.Name, GetAttributes(type))
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
                GetAttributes(field)
            )
        );

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    private static ImmutableList<AttributeModel>? GetAttributes(FieldDefinition field)
    {
        var attributes = ImmutableList.CreateRange(
            from a in field.CustomAttributes
            select a.Accept(AttributeVisitor.Instance)
        );

        return attributes.IsEmpty ? null : attributes;
    }
}

class UnionVisitor : StructVisitor
{
    public new static readonly UnionVisitor Instance = new();

    private UnionVisitor() {}

    public override UnionModel Visit(TypeDefinition type) =>
        new(type.Name, GetAttributes(type))
        {
            Nesting = GetNesting(type),
            Fields = GetFields(type)
        };

    protected override IImmutableList<AttributeModel>? GetAttributes(TypeDefinition type)
    {
        var attributes = base.GetAttributes(type);
        return attributes == null ? null
            : ImmutableList.CreateRange(
                from a in base.GetAttributes(type)
                where !IsStructLayout(a)
                select a
            );
    }

    private static bool IsStructLayout(AttributeModel attribute) =>
        attribute is { Name: "StructLayout", Namespace: "System.Runtime.InteropServices" };
}
