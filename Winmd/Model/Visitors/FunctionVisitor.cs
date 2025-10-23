using System.Collections.Immutable;
using Mono.Cecil;
using Winmd.ClassExtensions;

namespace Winmd.Model.Visitors;

internal class FunctionVisitor : IVisitor<MethodDefinition, FunctionModel?>
{
    public static readonly FunctionVisitor Instance = new();

    private FunctionVisitor() {}
    public static ImmutableHashSet<string> Interfaces { get; set; } = ImmutableHashSet<string>.Empty;

    public FunctionModel? Visit(MethodDefinition method)
    {
        var isCom = method.Parameters
            .Select(p => p.ParameterType.FullName)
            .Concat([method.MethodReturnType.ReturnType.FullName])
            .Any(Interfaces.Contains);
        if (isCom)
        {
            // TODO: Functions with COM parameters
            return null;
        }
        
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
