namespace Winmd.Model;

using System.Collections.Immutable;

class InterfaceModel(string name, IImmutableList<AttributeModel>? attributes) : BaseObjectModel(name, attributes)
{
    public override ModelType Type => ModelType.Interface;
    // TODO: Functions
}
