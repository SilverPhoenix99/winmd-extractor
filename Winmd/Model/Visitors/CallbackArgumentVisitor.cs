namespace Winmd.Model.Visitors;

using ClassExtensions;
using Mono.Cecil;

class CallbackArgumentVisitor : IVisitor<ParameterDefinition, CallbackArgumentModel>
{
    public static readonly CallbackArgumentVisitor Instance = new();

    private CallbackArgumentVisitor() {}

    public CallbackArgumentModel Visit(ParameterDefinition value) =>
        new(
            value.Name,
            value.ParameterType.Accept(TypeVisitor.Instance)
        )
        {
            Attributes = value.Attributes.Accept(FlagsEnumVisitor.Instance)
        };
}
