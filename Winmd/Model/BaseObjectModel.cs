namespace Winmd.Model;

using System.Collections.Immutable;

abstract class BaseObjectModel(string name, IImmutableList<AttributeModel>? attributes)
{
    public abstract ModelKind Kind { get; }
    public string Name => name;
    public IImmutableList<string>? Nesting { get; init; }
    public IImmutableList<AttributeModel>? Attributes => attributes;
}

class ObjectModel(ModelKind kind, string name, IImmutableList<AttributeModel>? attributes)
    : BaseObjectModel(name, attributes)
{
    public override ModelKind Kind => kind;
}
