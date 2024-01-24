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
            var lastModifier = modifiers.LastOrDefault();

            if (type.IsPointer)
            {
                if (lastModifier?.Pointer is not null)
                {
                    lastModifier.Pointer++;
                }
                else
                {
                    modifiers.Add(new TypeModifier { Pointer = 1 });
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
