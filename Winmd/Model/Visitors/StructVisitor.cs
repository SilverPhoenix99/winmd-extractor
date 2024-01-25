namespace Winmd.Model.Visitors;

using Mono.Cecil;

class StructVisitor : BaseObjectVisitor<StructModel>
{
    public static readonly StructVisitor Instance = new();

    private StructVisitor() {}

    public override StructModel Visit(TypeDefinition type)
    {
        // TODO: Fields
        // TODO: Nested classes & anonymous structs/unions

        return new StructModel(type.Name, GetAttributes(type));
    }
}

class UnionVisitor : BaseObjectVisitor<UnionModel>
{
    public static readonly UnionVisitor Instance = new();

    private UnionVisitor() {}

    public override UnionModel Visit(TypeDefinition type)
    {
        // TODO: Remove attribute StructLayout(LayoutKind.Explicit) [default]
        // TODO: Remove attribute FieldOffset(0) from fields

        return new UnionModel(type.Name, GetAttributes(type));
    }
}
