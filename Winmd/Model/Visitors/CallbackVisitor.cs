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

        model.ReturnType = method.ReturnType.Accept(TypeVisitor.Instance);

        var returnAttributes = method.MethodReturnType.CustomAttributes
            .Select(a => a.Accept(AttributeVisitor.Instance))
            .ToImmutableList();

        if (!returnAttributes.IsEmpty)
        {
            model.ReturnType.Attributes = returnAttributes;
        }

        var args =
            from p in method.Parameters
            select p.Accept(CallbackArgumentVisitor.Instance);

        model.Arguments = args.ToImmutableList();

        return model;
    }
}
