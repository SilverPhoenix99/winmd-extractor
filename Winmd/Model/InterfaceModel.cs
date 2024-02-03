namespace Winmd.Model;

using System.Collections.Immutable;

class InterfaceModel(string name, IImmutableList<AttributeModel>? attributes) : BaseObjectModel(name, attributes)
{
    public override ModelKind Kind => ModelKind.Interface;
    // TODO: Functions
}
