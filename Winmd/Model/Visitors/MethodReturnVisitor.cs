namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class MethodReturnVisitor : IVisitor<MethodReturnType, ReturnModel>
{
    public static readonly MethodReturnVisitor Instance = new();

    private MethodReturnVisitor() {}

    public ReturnModel Visit(MethodReturnType @return)
    {
        var type = @return.ReturnType.Accept(TypeVisitor.Instance);

        var returnAttributes = ImmutableList.CreateRange(
            from a in @return.CustomAttributes
            select a.Accept(AttributeVisitor.Instance)
        );

        return new ReturnModel(
            type,
            returnAttributes.IsEmpty ? null : returnAttributes
        );
    }
}
