namespace Winmd.Model;

using System.Collections.Immutable;

abstract class BaseObjectModel(string name, IImmutableList<AnnotationModel>? annotations)
{
    public abstract ModelKind Kind { get; }
    public string Name => name;
    public IImmutableList<string>? Nesting { get; init; }
    public IImmutableList<AnnotationModel>? Annotations => annotations;
}

class ObjectModel(ModelKind kind, string name, IImmutableList<AnnotationModel>? annotations)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => kind;
}
