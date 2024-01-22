namespace Winmd.Model.Visitors;

class ObjectVisitor(ModelType modelType) : BaseObjectVisitor<ObjectModel>
{
    protected override ObjectModel CreateModel(string name) => new(modelType, name);
}
