using System.Collections.Immutable;
using Mono.Cecil;
using Winmd.ClassExtensions;

namespace Winmd.Model.Visitors;

internal class TypeVisitor : IVisitor<TypeReference, TypeModel>
{
    public static ImmutableHashSet<string> ComInterfaces { get; set; } = ImmutableHashSet<string>.Empty;
    
    public static readonly TypeVisitor Instance = new();

    public static bool IsCom(TypeReference type) => ComInterfaces.Contains(type.FullName);

    private TypeVisitor() {}

    public TypeModel Visit(TypeReference type)
    {
        var (elementType, modifiers) = GetModifiers(type);
        return new TypeModel(
            elementType.Name,
            elementType.GetNamespace(),
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
