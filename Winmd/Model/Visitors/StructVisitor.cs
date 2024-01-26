namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ClassExtensions;
using Mono.Cecil;

class StructVisitor : BaseObjectVisitor<StructModel>
{
    public static readonly StructVisitor Instance = new();

    protected StructVisitor() {}

    public override StructModel Visit(TypeDefinition type)
    {
        return new StructModel(type.Name, GetAttributes(type))
        {
            Nesting = GetNesting(type),
            Fields = GetFields(type)
        };
    }

    protected static IImmutableList<string>? GetNesting(TypeDefinition type) =>
        type.GetNesting()?.Select(t => t.Name).ToImmutableList();

    protected virtual ImmutableList<FieldModel> GetFields(TypeDefinition type) =>
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
        // TODO: Remove attribute StructLayout(LayoutKind.Explicit) [default]
        return base.GetAttributes(type);
    }

    protected override ImmutableList<FieldModel> GetFields(TypeDefinition type)
    {
        // TODO: Remove attribute FieldOffset(0) from fields
        return base.GetFields(type);
    }
}
