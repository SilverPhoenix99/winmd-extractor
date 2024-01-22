namespace Winmd.Model;

class AttributeArgumentModel
{
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

    public string? Name { get; }
    public object? Value { get; }
    public TypeModel? Type { get; }
}
