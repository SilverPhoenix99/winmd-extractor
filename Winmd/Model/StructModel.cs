namespace Winmd.Model;

class StructModel(string name) : BaseObjectModel(name)
{
    public override ModelType Type => ModelType.Struct;
    // TODO: Fields
}

class UnionModel(string name) : StructModel(name)
{
    public override ModelType Type => ModelType.Union;
}
