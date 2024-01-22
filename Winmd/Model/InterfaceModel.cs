namespace Winmd.Model;

class InterfaceModel(string name) : BaseObjectModel(name)
{
    public override ModelType @Type => ModelType.Interface;
}
