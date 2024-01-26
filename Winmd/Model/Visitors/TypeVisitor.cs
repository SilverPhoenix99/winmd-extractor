namespace Winmd.Model.Visitors;

using System.Collections.Immutable;
using ClassExtensions;
using Mono.Cecil;

class TypeVisitor : IVisitor<TypeReference, TypeModel>
{
    public static readonly TypeVisitor Instance = new();

    private TypeVisitor() {}

    public TypeModel Visit(TypeReference type)
    {
        var (elementType, modifiers) = GetModifiers(type);
        var @namespace = elementType.GetNamespace();
        return new TypeModel(
            elementType.Name,
            @namespace != "System" ? @namespace : null,
            elementType.GetNesting()?.Select(t => t.Name).ToImmutableList(),
            modifiers
        );
    }

    private static (TypeReference, ImmutableList<TypeModifier>?) GetModifiers(TypeReference type)
    {
        var modifiers = new List<TypeModifier>();

        for (; type.IsPointer || type.IsArray; type = ((TypeSpecification) type).ElementType)
        {
            if (type.IsPointer)
            {
                var lastModifier = modifiers.LastOrDefault();
                if (lastModifier is not null)
                {
                    // Increment, and replace last element
                    modifiers[^1] = lastModifier with { Pointer = lastModifier.Pointer + 1 };
                }
                else
                {
                    modifiers.Add(new TypeModifier(1, null));
                }
            }
            else { // IsArray
                modifiers.AddRange(
                    from d in ((ArrayType) type).Dimensions
                    let len = d.Length() ?? -1
                    select new TypeModifier(null, len)
                );
            }
        }

        modifiers.Reverse();
        return (
            type,
            modifiers.IsEmpty() ? null : modifiers.ToImmutableList()
        );
    }

}
