namespace Winmd.Model.Visitors;

using ClassExtensions;
using Mono.Cecil;

class FunctionArgumentVisitor : IVisitor<ParameterDefinition, FunctionArgumentModel>
{
    public static readonly FunctionArgumentVisitor Instance = new();

    private FunctionArgumentVisitor() {}

    public FunctionArgumentModel Visit(ParameterDefinition parameter) =>
        new(
            parameter.Name,
            parameter.ParameterType.Accept(TypeVisitor.Instance)
        )
        {
            Attributes = parameter.Attributes.Accept(FlagsEnumVisitor.Instance)
        };
}
