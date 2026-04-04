using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Winmd.Model;

internal class InterfaceModel(string name, IImmutableList<AnnotationModel>? annotations) : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Interface;

    [UsedImplicitly]
    public TypeModel? Parent { get; init; }

    [UsedImplicitly]
    public IList<FunctionModel> Methods { get; init; } = ImmutableList<FunctionModel>.Empty;
}
