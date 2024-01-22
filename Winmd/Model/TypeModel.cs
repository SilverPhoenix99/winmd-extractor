namespace Winmd.Model;

class TypeModel(string name)
{
    public string Name => name;
    public string? Namespace { get; set; }
    public TypeModifier[]? Modifiers { get; set; }
}

class TypeModifier
{
    public int? Pointer { get; set; }
    public int? Array { get; set; }
}
