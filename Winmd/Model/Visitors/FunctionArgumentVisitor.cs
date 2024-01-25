namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class FunctionArgumentVisitor : IVisitor<ParameterDefinition, FunctionArgumentModel>
{
    public static readonly FunctionArgumentVisitor Instance = new();

    private FunctionArgumentVisitor() {}

    public FunctionArgumentModel Visit(ParameterDefinition parameter) =>
        new(
            parameter.Name,
            parameter.ParameterType.Accept(TypeVisitor.Instance),
            GetAttributes(parameter)
        );

    private static ImmutableList<AttributeModel>? GetAttributes(ParameterDefinition parameter)
    {
        var attributes = new List<AttributeModel>(
            from a in parameter.CustomAttributes
            select a.Accept(AttributeVisitor.Instance)
        );

        if (!IsDefault(parameter.Attributes))
        {
            attributes.Insert(0, new AttributeModel("ParameterFlags")
            {
                Arguments = ImmutableList.CreateRange(
                    from f in parameter.Attributes.Accept(FlagsEnumVisitor.Instance)
                    select new AttributeArgumentModel(f.ToString(), TypeModel.StringType)
                )
            });
        }

        return attributes.IsEmpty() ? null : attributes.ToImmutableList();
    }

    private static bool IsDefault(ParameterAttributes attributes) =>
        attributes is ParameterAttributes.None or ParameterAttributes.In;
}
