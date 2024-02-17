namespace Winmd.Model;

using System.Collections.Immutable;

class FunctionModel(string name, IImmutableList<AnnotationModel>? annotations, ReturnModel @return)
    : BaseObjectModel(name, annotations)
{
    public override ModelKind Kind => ModelKind.Function;
    public ReturnModel Return => @return;
    public IImmutableList<FunctionArgumentModel> Arguments { get; init; } = ImmutableList<FunctionArgumentModel>.Empty;
}

class CallbackModel(string name, IImmutableList<AnnotationModel>? annotations, ReturnModel @return)
    : FunctionModel(name, annotations, @return)
{
    public override ModelKind Kind => ModelKind.Callback;
}

record FunctionArgumentModel(string Name, TypeModel Type, IImmutableList<AnnotationModel>? Annotations = null);

record ReturnModel(TypeModel Type, IImmutableList<AnnotationModel>? Annotations = null);
