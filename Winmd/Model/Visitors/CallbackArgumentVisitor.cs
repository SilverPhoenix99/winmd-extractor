namespace Winmd.Model.Visitors;

using ClassExtensions;
using Mono.Cecil;

class CallbackArgumentVisitor : IVisitor<ParameterDefinition, CallbackArgumentModel>
{
    public static readonly CallbackArgumentVisitor Instance = new();

    private CallbackArgumentVisitor() {}

    public CallbackArgumentModel Visit(ParameterDefinition parameter) =>
        new(
            parameter.Name,
            parameter.ParameterType.Accept(TypeVisitor.Instance)
        )
        {
            Attributes = parameter.Attributes.Accept(FlagsEnumVisitor.Instance)
        };
}
