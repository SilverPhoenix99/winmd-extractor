using System.Collections.Immutable;

namespace Winmd.Model;

internal class StructModel(string name, IImmutableList<AnnotationModel>? annotations) : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Struct;
    public IImmutableList<FieldModel> Fields { get; init; } = ImmutableList<FieldModel>.Empty;
}

internal class UnionModel(string name, IImmutableList<AnnotationModel>? annotations) : StructModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Union;
}

internal record FieldModel(string Name, TypeModel Type, IImmutableList<AnnotationModel>? Annotations = null);
