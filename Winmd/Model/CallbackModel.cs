namespace Winmd.Model;

using System.Collections.Immutable;

class CallbackModel(string name) : BaseObjectModel(name)
{
    public override ModelType Type => ModelType.Callback;
    public TypeModel ReturnType { get; set; } = null!;
    public IImmutableList<CallbackArgumentModel> Arguments { get; set; } = ImmutableList<CallbackArgumentModel>.Empty;
}
