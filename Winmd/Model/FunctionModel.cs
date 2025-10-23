using System.Collections.Immutable;

namespace Winmd.Model;

internal class FunctionModel(string name, IImmutableList<AnnotationModel>? annotations, ReturnModel @return)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Function;
    public ReturnModel Return => @return;
    public IImmutableList<FunctionArgumentModel> Arguments { get; init; } = ImmutableList<FunctionArgumentModel>.Empty;
}

internal class CallbackModel(string name, IImmutableList<AnnotationModel>? annotations, ReturnModel @return)
    : FunctionModel(name, annotations, @return)
{
    public override ModelKind Kind => ModelKind.Callback;
}

internal record FunctionArgumentModel(string Name, TypeModel Type, IImmutableList<AnnotationModel>? Annotations = null);

internal record ReturnModel(TypeModel Type, IImmutableList<AnnotationModel>? Annotations = null);
