namespace Winmd.Model;

class TypedefModel(string name) : BaseObjectModel(name)
{
    public override ModelType @Type => ModelType.Typedef;
    public TypeModel SourceType { get; set; } = null!;
}
