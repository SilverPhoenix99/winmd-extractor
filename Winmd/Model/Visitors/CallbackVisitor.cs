namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class CallbackVisitor : BaseObjectVisitor<CallbackModel>
{
    public static readonly CallbackVisitor Instance = new();

    private CallbackVisitor() {}

    protected override CallbackModel CreateModel(string name) => new(name);

    public override CallbackModel Visit(TypeDefinition type)
    {
        var method = type.Methods.First(m => !m.IsConstructor && m.Name == "Invoke")!;

        var model = base.Visit(type);

        model.Return = method.MethodReturnType.Accept(MethodReturnVisitor.Instance);

        model.Arguments = method.Parameters
            .Select(p => p.Accept(FunctionArgumentVisitor.Instance))
            .ToImmutableList();

        return model;
    }
}
