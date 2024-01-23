namespace Winmd.Model;

class ConstantModel(string name, TypeModel constantType) : BaseObjectModel(name)
{
    public override ModelType Type => ModelType.Constant;
    public TypeModel ConstantType => constantType;
    public object Value { get; set; } = null!;
    public TypeModel ValueType { get; set; } = null!;
}
