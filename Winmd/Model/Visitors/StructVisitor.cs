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
        // TODO: Nested classes & anonymous structs/unions

        return new StructModel(type.Name, GetAttributes(type))
        {
            Fields = GetFields(type)
        };
    }

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

    public override UnionModel Visit(TypeDefinition type)
    {
        // TODO: Remove attribute StructLayout(LayoutKind.Explicit) [default]
        // TODO: Remove attribute FieldOffset(0) from fields

        return new UnionModel(type.Name, GetAttributes(type))
        {
            Fields = GetFields(type)
        };
    }
}
