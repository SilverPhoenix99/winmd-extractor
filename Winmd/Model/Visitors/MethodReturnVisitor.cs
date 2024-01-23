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

        var returnAttributes = @return.CustomAttributes
            .Select(a => a.Accept(AttributeVisitor.Instance))
            .ToImmutableList();

        return new ReturnModel(type)
        {
            Attributes = returnAttributes.IsEmpty ? null : returnAttributes
        };
    }
}
