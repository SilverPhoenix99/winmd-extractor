using Mono.Cecil;
using Winmd.Model;

namespace Winmd.Visitors;

internal class ObjectVisitor(ModelKind kind) : BaseObjectVisitor<ObjectModel>
{
    public override ObjectModel Visit(TypeDefinition type) => new(kind, type.Name, GetAnnotations(type));
}
