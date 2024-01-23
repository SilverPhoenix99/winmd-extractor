namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class FunctionArgumentVisitor : IVisitor<ParameterDefinition, FunctionArgumentModel>
{
    public static readonly FunctionArgumentVisitor Instance = new();

    private FunctionArgumentVisitor() {}

    public FunctionArgumentModel Visit(ParameterDefinition parameter)
    {
        var attributes = new List<AttributeModel>();

        if (!IsDefault(parameter.Attributes))
        {
            attributes.Add(new AttributeModel("ParameterFlags")
            {
                Arguments = parameter.Attributes.Accept(FlagsEnumVisitor.Instance)
                    .Select(f => new AttributeArgumentModel(TypeModel.StringType, f.ToString()))
                    .ToImmutableList()
            });
        }

        return new FunctionArgumentModel(
            parameter.Name,
            parameter.ParameterType.Accept(TypeVisitor.Instance)
        )
        {
            Attributes = attributes.IsEmpty() ? null : attributes.ToImmutableList()
        };
    }

    private static bool IsDefault(ParameterAttributes attributes) =>
        attributes is ParameterAttributes.None or ParameterAttributes.In;
}
