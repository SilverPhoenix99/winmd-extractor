namespace Winmd.Model;

class CustomAttributeArgumentModel(TypeModel type, object? value, string? name = null)
{
    public string? Name => name;
    public object? Value => value;
    public TypeModel Type => type;
}
