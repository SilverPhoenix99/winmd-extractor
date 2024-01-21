namespace Winmd.Model;

using System.Collections.Immutable;

class CustomAttributeModel(string name)
{
    public string Name => name;
    public string? Namespace { get; set; }
    public IImmutableList<CustomAttributeArgumentModel> Arguments { get; set; } = ImmutableArray<CustomAttributeArgumentModel>.Empty;
}
