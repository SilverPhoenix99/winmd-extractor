using System.Collections.Immutable;

namespace Winmd.Model;

internal class EnumModel(string name, IImmutableList<AnnotationModel>? annotations) : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Enum;
    public string? EnumType { get; init; }
    public IImmutableList<EnumMemberModel> Members { get; init; } = ImmutableList<EnumMemberModel>.Empty;
}

internal record EnumMemberModel(string Name, object? Constant);
