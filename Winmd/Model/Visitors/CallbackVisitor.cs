namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class CallbackVisitor : BaseObjectModelVisitor<CallbackModel>
{
    public static readonly CallbackVisitor Instance = new();

    private CallbackVisitor() {}

    protected override CallbackModel CreateModel(string name) => new(name);

    public override CallbackModel Visit(TypeDefinition value)
    {
        var method = value.Methods.First(m => !m.IsConstructor && m.Name == "Invoke")!;

        var model = base.Visit(value);

        model.ReturnType = method.ReturnType.Resolve().Accept(TypeModelVisitor.Instance);

        var args =
            from p in method.Parameters
            select p.Accept(CallbackArgumentVisitor.Instance);

        model.Arguments = args.ToImmutableList();

        return model;
    }
}
