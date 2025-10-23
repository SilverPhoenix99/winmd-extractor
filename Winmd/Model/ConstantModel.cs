using System.Collections.Immutable;

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
    public TypeModel ConstantType => constantType;
    public object Value => value;
    public TypeModel ValueType => valueType;
}
