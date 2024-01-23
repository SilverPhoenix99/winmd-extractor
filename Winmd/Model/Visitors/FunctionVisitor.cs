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
            .ToList();

        var pInvokeAttribute = method.Accept(PInvokeInfoVisitor.Instance);
        if (pInvokeAttribute != null)
        {
            attributes.Insert(0, pInvokeAttribute);
        }

        return new FunctionModel(method.Name)
        {
            Attributes = attributes.IsEmpty() ? null : attributes.ToImmutableList(),
            Return = method.MethodReturnType.Accept(MethodReturnVisitor.Instance),
            Arguments = method.Parameters
                .Select(arg => arg.Accept(FunctionArgumentVisitor.Instance))
                .ToImmutableList()
        };
    }
}
