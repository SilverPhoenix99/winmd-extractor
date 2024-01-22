namespace Winmd.Model;

using System.Collections.Immutable;

abstract class BaseObjectModel(string name)
{
    public abstract ModelType Type { get; }
    public string Name => name;
    public IImmutableList<AttributeModel> Attributes { get; set; } = ImmutableList<AttributeModel>.Empty;
}

class ObjectModel(ModelType type, string name) : BaseObjectModel(name)
{
    public override ModelType Type => type;
}
