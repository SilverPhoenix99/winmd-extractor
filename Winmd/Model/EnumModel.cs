namespace Winmd.Model;

using System.Collections.Immutable;

class EnumModel(string name, IImmutableList<AttributeModel>? attributes) : BaseObjectModel(name, attributes)
{
    public override ModelKind Kind => ModelKind.Enum;
    public string? EnumType { get; init; }
    public IImmutableList<EnumMemberModel> Members { get; init; } = ImmutableList<EnumMemberModel>.Empty;
}

record EnumMemberModel(string Name, object? Constant);
