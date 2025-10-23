using Mono.Cecil;

namespace Winmd.Model.Visitors;

internal class ObjectVisitor(ModelKind kind) : BaseObjectVisitor<ObjectModel>
{
    public override ObjectModel Visit(TypeDefinition type) => new(kind, type.Name, GetAnnotations(type));
}
