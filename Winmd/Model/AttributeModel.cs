namespace Winmd.Model;

using System.Collections.Immutable;

class AttributeModel(string name)
{
    public string Name => name;
    public string? Namespace { get; set; }
    public IImmutableList<AttributeArgumentModel> Arguments { get; set; } = ImmutableArray<AttributeArgumentModel>.Empty;
    public IDictionary<string, object> Properties { get; set; } = ImmutableDictionary<string, object>.Empty;
}

class AttributeArgumentModel(TypeModel type, object? value)
{
    public object? Value => value;
    public TypeModel Type => type;
}
