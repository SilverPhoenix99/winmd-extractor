namespace Winmd.Model;

using System.Collections.Immutable;

class InterfaceModel(string name, IImmutableList<AnnotationModel>? annotations) : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Interface;
    // TODO: Functions
}
