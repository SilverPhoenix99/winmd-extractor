namespace Winmd.Model.Visitors;

using Mono.Cecil;

class StructVisitor : BaseObjectVisitor<StructModel>
{
    public static readonly StructVisitor Instance = new();

    private StructVisitor() {}

    public override StructModel Visit(TypeDefinition type)
    {
        var model = base.Visit(type);

        // TODO: Fields
        // TODO: Nested classes
        // TODO: anonymous structs/unions

        return model;
    }

    protected override StructModel CreateModel(string name) => new(name);
}
