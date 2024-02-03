namespace Winmd.Model;

using System.Collections.Immutable;

class ConstantModel(
    string name,
    TypeModel constantType,
    IImmutableList<AttributeModel>? attributes,
    object value,
    TypeModel valueType
)
    : BaseObjectModel(name, attributes)
{
    public override ModelKind Kind => ModelKind.Constant;
    public TypeModel ConstantType => constantType;
    public object Value => value;
    public TypeModel ValueType => valueType;
}
