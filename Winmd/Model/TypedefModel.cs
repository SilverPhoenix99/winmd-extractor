namespace Winmd.Model;

using System.Collections.Immutable;

class TypedefModel(string name, IImmutableList<AnnotationModel>? annotations, TypeModel sourceType)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Typedef;
    public TypeModel SourceType => sourceType;
}
