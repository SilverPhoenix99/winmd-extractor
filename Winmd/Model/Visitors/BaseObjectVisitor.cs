namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

abstract class BaseObjectVisitor<T> : IVisitor<TypeDefinition, T>
    where T : BaseObjectModel
{
    public abstract T Visit(TypeDefinition type);

    protected virtual IImmutableList<AttributeModel>? GetAttributes(TypeDefinition type)
    {
        var structLayout = type.Accept(StructLayoutVisitor.Instance);

        var attributes = Visit(type.CustomAttributes);
        if (structLayout is not null)
        {
            attributes.Insert(0, structLayout);
        }

        return attributes.IsEmpty() ? null : attributes.ToImmutableList();
    }

    private static List<AttributeModel> Visit(IEnumerable<CustomAttribute> customAttributes) =>
    [
        ..from a in customAttributes
        select a.Accept(AttributeVisitor.Instance)
    ];
}
