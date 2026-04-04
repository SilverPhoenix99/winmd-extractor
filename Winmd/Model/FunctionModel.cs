using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Winmd.Model;

internal class FunctionModel(
    string name,
    IImmutableList<AnnotationModel>? annotations,
    ReturnModel @return
)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Function;

    [UsedImplicitly]
    public ReturnModel Return => @return;

    [UsedImplicitly]
    public IImmutableList<FunctionArgumentModel> Arguments { get; init; } = ImmutableList<FunctionArgumentModel>.Empty;
}

internal class CallbackModel(
    string name,
    IImmutableList<AnnotationModel>? annotations,
    ReturnModel @return
)
    : FunctionModel(name, annotations, @return)
{
    public override ModelKind Kind => ModelKind.Callback;
}

internal record FunctionArgumentModel(
    [UsedImplicitly] string Name,
    [UsedImplicitly] TypeModel Type,
    [UsedImplicitly] IImmutableList<AnnotationModel>? Annotations = null
);

internal record ReturnModel(
    [UsedImplicitly] TypeModel Type,
    [UsedImplicitly] IImmutableList<AnnotationModel>? Annotations = null
);
