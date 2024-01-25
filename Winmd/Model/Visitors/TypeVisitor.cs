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
        var modifiers = new List<TypeModifier>();

        for (; type.IsPointer || type.IsArray; type = ((TypeSpecification) type).ElementType)
        {
            if (type.IsPointer)
            {
                var pointer = modifiers.LastOrDefault()?.Pointer;
                if (pointer is not null)
                {
                    // Increment, and replace last element
                    modifiers[^1] = new TypeModifier(pointer + 1, null);
                }
                else
                {
                    modifiers.Add(new TypeModifier(1, null));
                }
            }
            else {
                // IsArray
                // var dimensions = ((ArrayType) type).Dimensions;
                throw new NotImplementedException("TODO: Array Types");
            }
        }

        modifiers.Reverse();

        return new TypeModel(
            type.Name,
            type.Namespace != "System" ? type.Namespace : null
        )
        {
            Modifiers = modifiers.IsEmpty() ? null : modifiers.ToImmutableList()
        };
    }
}
