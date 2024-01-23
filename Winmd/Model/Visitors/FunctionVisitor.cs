namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class FunctionVisitor : IVisitor<MethodDefinition, FunctionModel>
{
    public static readonly FunctionVisitor Instance = new();

    private FunctionVisitor() {}

    public FunctionModel Visit(MethodDefinition method)
    {
        var attributes = method.CustomAttributes
            .Select(a => a.Accept(AttributeVisitor.Instance))
            .ToImmutableList();

        // TODO: method.PInvokeInfo -> DllImport attribute
        // TODO: method.MethodReturnType.CustomAttributes

        return new FunctionModel(method.Name)
        {
            Attributes = attributes.IsEmpty ? null : attributes,
            ReturnType = method.ReturnType.Accept(TypeVisitor.Instance),
            Arguments = method.Parameters
                .Select(arg => arg.Accept(CallbackArgumentVisitor.Instance))
                .ToImmutableList()
        };
    }
}
