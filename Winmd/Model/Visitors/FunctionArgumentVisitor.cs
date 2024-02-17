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
            GetAnnotations(parameter)
        );

    private static ImmutableList<AnnotationModel>? GetAnnotations(ParameterDefinition parameter)
    {
        var annotations = new List<AnnotationModel>(
            from a in parameter.CustomAttributes
            select a.Accept(AnnotationVisitor.Instance)
        );

        if (!IsDefault(parameter.Attributes))
        {
            annotations.Insert(0, new AnnotationModel("ParameterFlags")
            {
                Arguments = ImmutableList.CreateRange(
                    from f in parameter.Attributes.Accept(FlagsEnumVisitor.Instance)
                    select new AnnotationArgumentModel(f.ToString(), TypeModel.StringType)
                )
            });
        }

        return annotations.IsEmpty() ? null : annotations.ToImmutableList();
    }

    private static bool IsDefault(ParameterAttributes attributes) =>
        attributes is ParameterAttributes.None or ParameterAttributes.In;
}
