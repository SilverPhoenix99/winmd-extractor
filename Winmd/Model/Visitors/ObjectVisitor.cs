namespace Winmd.Model.Visitors;

using Mono.Cecil;

class ObjectVisitor(ModelType modelType) : BaseObjectVisitor<ObjectModel>
{
    public override ObjectModel Visit(TypeDefinition type) => new(modelType, type.Name, GetAttributes(type));
}
