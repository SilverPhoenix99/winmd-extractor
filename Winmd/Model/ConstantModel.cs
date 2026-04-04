using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Winmd.Model;

internal class ConstantModel(
    string name,
    TypeModel constantType,
    IImmutableList<AnnotationModel>? annotations,
    object value,
    TypeModel valueType
)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Constant;

    [UsedImplicitly]
    public TypeModel ConstantType => constantType;

    [UsedImplicitly]
    public object Value => value;

    [UsedImplicitly]
    public TypeModel ValueType => valueType;
}
