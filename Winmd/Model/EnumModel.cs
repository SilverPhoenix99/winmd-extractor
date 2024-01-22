namespace Winmd.Model;

using System.Collections.Immutable;

class EnumModel(string name) : BaseObjectModel(name)
{
    public override ModelType Type => ModelType.Enum;
    public string? EnumType { get; set; }
    public IImmutableList<EnumMemberModel> Members { get; set; } = ImmutableList<EnumMemberModel>.Empty;
}
