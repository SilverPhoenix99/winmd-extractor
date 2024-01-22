namespace Winmd.Model;

using System.Collections.Immutable;

class AttributeModel(string name)
{
    public string Name => name;
    public string? Namespace { get; set; }
    public IImmutableList<AttributeArgumentModel> Arguments { get; set; } = ImmutableArray<AttributeArgumentModel>.Empty;
}
