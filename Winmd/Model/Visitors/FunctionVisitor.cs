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
        var annotations = new List<AnnotationModel>(
            from a in method.CustomAttributes
            select a.Accept(AnnotationVisitor.Instance)
        );

        var pInvokeAttribute = method.Accept(PInvokeInfoVisitor.Instance);
        if (pInvokeAttribute is not null)
        {
            annotations.Insert(0, pInvokeAttribute);
        }

        var @return = method.MethodReturnType.Accept(MethodReturnVisitor.Instance);

        return new FunctionModel(
            method.Name,
            annotations.IsEmpty() ? null : annotations.ToImmutableList(),
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
