namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

abstract class BaseObjectVisitor<T> : IVisitor<TypeDefinition, T>
    where T : BaseObjectModel
{
    public abstract T Visit(TypeDefinition type);

    protected virtual IImmutableList<AnnotationModel>? GetAnnotations(TypeDefinition type)
    {
        var structLayout = type.Accept(StructLayoutVisitor.Instance);

        var annotations = Visit(type.CustomAttributes);
        if (structLayout is not null)
        {
            annotations.Insert(0, structLayout);
        }

        return annotations.IsEmpty() ? null : annotations.ToImmutableList();
    }

    private static List<AnnotationModel> Visit(IEnumerable<CustomAttribute> customAttributes) =>
    [..
        from a in customAttributes
        select a.Accept(AnnotationVisitor.Instance)
    ];
}
