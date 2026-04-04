using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Winmd.Model;

internal class EnumModel(
    string name,
    IImmutableList<AnnotationModel>? annotations
)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Enum;

    [UsedImplicitly]
    public string? EnumType { get; init; }

    [UsedImplicitly]
    public IImmutableList<EnumMemberModel> Members { get; init; } = ImmutableList<EnumMemberModel>.Empty;
}

internal record EnumMemberModel(
    [UsedImplicitly] string Name,
    [UsedImplicitly] object? Constant
);
