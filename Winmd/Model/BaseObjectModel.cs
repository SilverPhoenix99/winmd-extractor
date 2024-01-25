namespace Winmd.Model;

using System.Collections.Immutable;

abstract class BaseObjectModel(string name, IImmutableList<AttributeModel>? attributes)
{
    public abstract ModelType Type { get; }
    public string Name => name;
    public IImmutableList<AttributeModel>? Attributes => attributes;
}

class ObjectModel(ModelType type, string name, IImmutableList<AttributeModel>? attributes)
    : BaseObjectModel(name, attributes)
{
    public override ModelType Type => type;
}
