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

        var returnType = method.ReturnType.Accept(TypeVisitor.Instance);

        var returnAttributes = method.MethodReturnType.CustomAttributes
            .Select(a => a.Accept(AttributeVisitor.Instance))
            .ToImmutableList();

        if (!returnAttributes.IsEmpty)
        {
            returnType.Attributes = returnAttributes;
        }

        // TODO: method.PInvokeInfo -> DllImport attribute

        return new FunctionModel(method.Name)
        {
            Attributes = attributes.IsEmpty ? null : attributes,
            ReturnType = method.ReturnType.Accept(TypeVisitor.Instance),
            Arguments = method.Parameters
                .Select(arg => arg.Accept(FunctionArgumentVisitor.Instance))
                .ToImmutableList()
        };
    }
}
