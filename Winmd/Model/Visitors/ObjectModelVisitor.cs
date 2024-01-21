namespace Winmd.Model.Visitors;

class ObjectModelVisitor(ModelType modelType) : BaseObjectModelVisitor<ObjectModel>
{
    protected override ObjectModel CreateModel(string name) => new(modelType, name);
}
