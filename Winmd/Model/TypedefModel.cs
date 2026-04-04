using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Winmd.Model;

internal class TypedefModel(
    string name,
    IImmutableList<AnnotationModel>? annotations,
    TypeModel sourceType
)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Typedef;

    [UsedImplicitly]
    public TypeModel SourceType => sourceType;
}
