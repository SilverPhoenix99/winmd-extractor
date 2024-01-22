namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

abstract class BaseObjectVisitor<T> : IVisitor<TypeDefinition, T>
    where T : BaseObjectModel
{
    protected abstract T CreateModel(string name);

    public virtual T Visit(TypeDefinition type)
    {
        var model = CreateModel(type.Name);

        var structLayout = type.Accept(StructLayoutVisitor.Instance);

        var attributes = Visit(type.CustomAttributes);
        if (structLayout is not null)
        {
            attributes.Insert(0, structLayout);
        }

        model.Attributes = attributes.ToImmutableList();

        return model;
    }

    private static List<AttributeModel> Visit(IEnumerable<CustomAttribute> customAttributes) =>
        customAttributes
            .Select(a => a.Accept(CustomAttributeVisitor.Instance))
            .ToList();
}
