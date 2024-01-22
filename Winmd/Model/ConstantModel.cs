namespace Winmd.Model;

class ConstantModel(string name) : BaseObjectModel(name)
{
    public override ModelType Type => ModelType.Constant;
    public TypeModel ConstantType { get; set; } = null!;
    public object Value { get; set; } = null!;
    public TypeModel ValueType { get; set; } = null!;
}
