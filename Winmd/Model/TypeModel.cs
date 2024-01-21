namespace Winmd.Model;

class TypeModel(string name)
{
    public string Name => name;
    public string? Namespace { get; set; }
    public int? Pointer { get; set; }
    public bool? Array { get; set; }
}
