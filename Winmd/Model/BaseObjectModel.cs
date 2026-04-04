using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Winmd.Model;

internal abstract class BaseObjectModel(
    string name,
    IImmutableList<AnnotationModel>? annotations
)
{
    [UsedImplicitly]
    public abstract ModelKind Kind { get; }

    [UsedImplicitly]
    public string Name => name;

    [UsedImplicitly]
    public IImmutableList<string>? Nesting { get; init; }

    [UsedImplicitly]
    public IImmutableList<AnnotationModel>? Annotations => annotations;
}

internal class ObjectModel(
    ModelKind kind,
    string name,
    IImmutableList<AnnotationModel>? annotations
)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => kind;
}
