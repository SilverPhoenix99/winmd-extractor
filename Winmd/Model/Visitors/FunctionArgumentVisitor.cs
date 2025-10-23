using System.Collections.Immutable;
using Mono.Cecil;
using Winmd.ClassExtensions;

namespace Winmd.Model.Visitors;

internal class FunctionArgumentVisitor : IVisitor<ParameterDefinition, FunctionArgumentModel>
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

        var flagsAnnotation = CreateFlags(parameter.Attributes);
        if (flagsAnnotation is not null)
        {
            annotations.Insert(0, flagsAnnotation);
        }

        return annotations.IsEmpty() ? null : annotations.ToImmutableList();
    }

    private static AnnotationModel? CreateFlags(ParameterAttributes attributes)
    {
        if (IsDefault(attributes))
        {
            return null;
        }

        string[] flags =
        [..
            from f in attributes.Accept(FlagsEnumVisitor.Instance)
            select f.ToString()
        ];

        return new AnnotationModel("ParameterFlags")
        {
            Properties = ImmutableDictionary<string, object>.Empty.Add("Value", flags)
        };
    }

    private static bool IsDefault(ParameterAttributes attributes) =>
        attributes is ParameterAttributes.None or ParameterAttributes.In;
}
