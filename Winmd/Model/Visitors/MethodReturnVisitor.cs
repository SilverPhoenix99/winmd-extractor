using System.Collections.Immutable;
using Mono.Cecil;
using Winmd.ClassExtensions;

namespace Winmd.Model.Visitors;

internal class MethodReturnVisitor : IVisitor<MethodReturnType, ReturnModel>
{
    public static readonly MethodReturnVisitor Instance = new();

    private MethodReturnVisitor() {}

    public ReturnModel Visit(MethodReturnType @return)
    {
        var type = @return.ReturnType.Accept(TypeVisitor.Instance);

        var annotations = ImmutableList.CreateRange(
            from a in @return.CustomAttributes
            select a.Accept(AnnotationVisitor.Instance)
        );

        return new ReturnModel(
            type,
            annotations.IsEmpty ? null : annotations
        );
    }
}
