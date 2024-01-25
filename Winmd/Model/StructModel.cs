namespace Winmd.Model;

using System.Collections.Immutable;

class StructModel(string name, IImmutableList<AttributeModel>? attributes) : BaseObjectModel(name, attributes)
{
    public override ModelType Type => ModelType.Struct;
    // TODO: Fields
}

class UnionModel(string name, IImmutableList<AttributeModel>? attributes) : StructModel(name, attributes)
{
    public override ModelType Type => ModelType.Union;
}
