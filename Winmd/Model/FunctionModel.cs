namespace Winmd.Model;

using System.Collections.Immutable;

class FunctionModel(string name) : BaseObjectModel(name)
{
    public override ModelType Type => ModelType.Function;
    public TypeModel ReturnType { get; set; } = null!;
    public IImmutableList<FunctionArgumentModel> Arguments { get; set; } = ImmutableList<FunctionArgumentModel>.Empty;
}

class CallbackModel(string name) : FunctionModel(name)
{
    public override ModelType Type => ModelType.Callback;
}
