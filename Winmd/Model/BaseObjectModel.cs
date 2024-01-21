namespace Winmd.Model;

using System.Collections.Immutable;

abstract class BaseObjectModel(string name)
{
    public abstract ModelType Type { get; }
    public string Name => name;
    public int? ClassSize { get; set; }
    public int? PackingSize { get; set; }
    public IImmutableSet<string> Attributes { get; set; } = ImmutableHashSet<string>.Empty;
    public IImmutableList<CustomAttributeModel> CustomAttributes { get; set; } = ImmutableList<CustomAttributeModel>.Empty;
}

class ObjectModel(ModelType type, string name) : BaseObjectModel(name)
{
    public override ModelType Type => type;
}
