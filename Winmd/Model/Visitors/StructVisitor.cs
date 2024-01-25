namespace Winmd.Model.Visitors;

using Mono.Cecil;

class StructVisitor : BaseObjectVisitor<StructModel>
{
    public static readonly StructVisitor Instance = new();

    protected StructVisitor() {}

    public override StructModel Visit(TypeDefinition type)
    {
        var model = base.Visit(type);

        // TODO: Fields
        // TODO: Nested classes & anonymous structs/unions

        return model;
    }

    protected override StructModel CreateModel(string name) => new(name);
}

class UnionVisitor : StructVisitor
{
    public new static readonly UnionVisitor Instance = new();

    private UnionVisitor() {}

    protected override StructModel CreateModel(string name) => new UnionModel(name);

    public override StructModel Visit(TypeDefinition type)
    {
        var model = base.Visit(type);

        // TODO: Remove attribute StructLayout(LayoutKind.Explicit) [default]
        // TODO: Remove attribute FieldOffset(0) from fields

        return model;
    }
}
