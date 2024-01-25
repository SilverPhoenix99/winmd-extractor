namespace Winmd.Model;

using System.Collections.Immutable;

class FunctionModel(string name, IImmutableList<AttributeModel>? attributes, ReturnModel @return)
    : BaseObjectModel(name, attributes)
{
    public override ModelType Type => ModelType.Function;
    public ReturnModel Return => @return;
    public IImmutableList<FunctionArgumentModel> Arguments { get; init; } = ImmutableList<FunctionArgumentModel>.Empty;
}

class CallbackModel(string name, IImmutableList<AttributeModel>? attributes, ReturnModel @return)
    : FunctionModel(name, attributes, @return)
{
    public override ModelType Type => ModelType.Callback;
}

// Preserved as class, due to field's order
class FunctionArgumentModel(string name, TypeModel type, IImmutableList<AttributeModel>? attributes)
{
    public string Name => name;
    public IImmutableList<AttributeModel>? Attributes = attributes;
    public TypeModel Type => type;
}

record ReturnModel(TypeModel Type, IImmutableList<AttributeModel>? Attributes = null);
