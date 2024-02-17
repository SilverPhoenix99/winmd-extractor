namespace Winmd.Model;

using System.Collections.Immutable;

class FunctionModel(string name, IImmutableList<AttributeModel>? attributes, ReturnModel @return)
    : BaseObjectModel(name, attributes)
{
    public override ModelKind Kind => ModelKind.Function;
    public ReturnModel Return => @return;
    public IImmutableList<FunctionArgumentModel> Arguments { get; init; } = ImmutableList<FunctionArgumentModel>.Empty;
}

class CallbackModel(string name, IImmutableList<AttributeModel>? attributes, ReturnModel @return)
    : FunctionModel(name, attributes, @return)
{
    public override ModelKind Kind => ModelKind.Callback;
}

record FunctionArgumentModel(string Name, TypeModel Type, IImmutableList<AttributeModel>? Attributes = null);

record ReturnModel(TypeModel Type, IImmutableList<AttributeModel>? Attributes = null);
