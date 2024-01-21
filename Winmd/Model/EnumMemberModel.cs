namespace Winmd.Model;

class EnumMemberModel(string name, object? constant)
{
    public string Name => name;
    public object? Constant => constant;
}
