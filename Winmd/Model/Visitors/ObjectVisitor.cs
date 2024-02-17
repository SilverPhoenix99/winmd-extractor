namespace Winmd.Model.Visitors;

using Mono.Cecil;

class ObjectVisitor(ModelKind kind) : BaseObjectVisitor<ObjectModel>
{
    public override ObjectModel Visit(TypeDefinition type) => new(kind, type.Name, GetAnnotations(type));
}
