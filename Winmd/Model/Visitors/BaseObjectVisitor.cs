namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

abstract class BaseObjectVisitor<T> : IVisitor<TypeDefinition, T>
    where T : BaseObjectModel
{
    protected abstract T CreateModel(string name);

    public virtual T Visit(TypeDefinition value)
    {
        var model = CreateModel(value.Name);
        model.Attributes = value.Attributes.Accept(FlagsEnumVisitor.Instance);
        model.ClassSize = value.ClassSize > 0 ? value.ClassSize : null;
        model.PackingSize = value.PackingSize > 0 ? value.PackingSize : null;
        model.CustomAttributes = Visit(value.CustomAttributes);

        return model;
    }

    private static ImmutableList<CustomAttributeModel> Visit(IEnumerable<CustomAttribute> customAttributes) =>
        customAttributes
            .Select(a => a.Accept(CustomAttributeVisitor.Instance))
            .ToImmutableList();
}
