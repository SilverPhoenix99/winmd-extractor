namespace Winmd.Model;

using System.Collections.Immutable;

class FunctionModel(string name) : BaseObjectModel(name)
{
    public override ModelType Type => ModelType.Function;
    public ReturnModel Return { get; set; } = null!;
    public IImmutableList<FunctionArgumentModel> Arguments { get; set; } = ImmutableList<FunctionArgumentModel>.Empty;
}

class CallbackModel(string name) : FunctionModel(name)
{
    public override ModelType Type => ModelType.Callback;
}

class FunctionArgumentModel(string name, TypeModel type)
{
    public string Name => name;
    public IImmutableList<AttributeModel>? Attributes { get; set; }
    public TypeModel Type => type;
}

class ReturnModel(TypeModel type)
{
    public TypeModel Type => type;
    public IImmutableList<AttributeModel>? Attributes { get; set; }
}
