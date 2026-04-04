using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Winmd.Model;

internal class StructModel(
    string name,
    IImmutableList<AnnotationModel>? annotations
)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Struct;

    [UsedImplicitly]
    public IImmutableList<FieldModel> Fields { get; init; } = ImmutableList<FieldModel>.Empty;
}

internal class UnionModel(
    string name,
    IImmutableList<AnnotationModel>? annotations
)
    : StructModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Union;
}

internal record FieldModel(
    [UsedImplicitly] string Name,
    [UsedImplicitly] TypeModel Type,
    [UsedImplicitly] IImmutableList<AnnotationModel>? Annotations = null
);
