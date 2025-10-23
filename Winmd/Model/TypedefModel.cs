using System.Collections.Immutable;

namespace Winmd.Model;

internal class TypedefModel(string name, IImmutableList<AnnotationModel>? annotations, TypeModel sourceType)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Typedef;
    public TypeModel SourceType => sourceType;
}
