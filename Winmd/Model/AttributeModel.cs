namespace Winmd.Model;

using System.Collections.Immutable;

class AttributeModel(string name)
{
    public string Name => name;
    public string? Namespace { get; set; }
    public IImmutableList<AttributeArgumentModel> Arguments { get; set; } = ImmutableArray<AttributeArgumentModel>.Empty;
}

class AttributeArgumentModel
{
    public string? Name { get; }
    public object? Value { get; }
    public TypeModel? Type { get; }

    public AttributeArgumentModel(TypeModel type, object? value)
    {
        Type = type;
        Value = value;
    }

    public AttributeArgumentModel(string name, object? value)
    {
        Name = name;
        Value = value;
    }
}
