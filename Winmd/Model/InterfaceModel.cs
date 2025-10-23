using System.Collections.Immutable;

namespace Winmd.Model;

internal class InterfaceModel(string name, IImmutableList<AnnotationModel>? annotations) : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Interface;
    // TODO: Functions
}
