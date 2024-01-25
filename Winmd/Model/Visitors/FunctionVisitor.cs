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
        var attributes = new List<AttributeModel>(
            from a in method.CustomAttributes
            select a.Accept(AttributeVisitor.Instance)
        );

        var pInvokeAttribute = method.Accept(PInvokeInfoVisitor.Instance);
        if (pInvokeAttribute is not null)
        {
            attributes.Insert(0, pInvokeAttribute);
        }

        var @return = method.MethodReturnType.Accept(MethodReturnVisitor.Instance);

        return new FunctionModel(
            method.Name,
            attributes.IsEmpty() ? null : attributes.ToImmutableList(),
            @return
        )
        {
            Arguments = ImmutableList.CreateRange(
                from arg in method.Parameters
                select arg.Accept(FunctionArgumentVisitor.Instance)
            )
        };
    }
}
