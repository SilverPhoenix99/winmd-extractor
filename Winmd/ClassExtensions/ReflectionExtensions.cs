using Mono.Cecil;

namespace Winmd.ClassExtensions;

internal static class ReflectionExtensions
{
    extension(Type type)
    {
        public (string Name, string? Namespace) GetQualifiedName(bool stripAttributes = true)
        {
            var name = type.Name;
            if (stripAttributes && type.BaseType == typeof(Attribute))
            {
                name = name.StripEnd("Attribute");
            }

            return (
                name,
                type.Namespace != "System" ? type.Namespace : null
            );
        }
    }

    extension(ArrayDimension dimension)
    {
        public int? Length() => dimension.IsSized
            ? dimension.UpperBound - dimension.LowerBound + 1
            : null;
    }

    extension(CustomAttribute element)
    {
        public TO Accept<TO>(IVisitor<CustomAttribute, TO> visitor) => visitor.Visit(element);
    }

    extension(Enum element)
    {
        public TO Accept<TO>(IVisitor<Enum, TO> visitor) => visitor.Visit(element);
    }

    extension(FieldDefinition element)
    {
        public TO Accept<TO>(IVisitor<FieldDefinition, TO> visitor) => visitor.Visit(element);
    }

    extension(ParameterDefinition element)
    {
        public TO Accept<TO>(IVisitor<ParameterDefinition, TO> visitor) => visitor.Visit(element);
    }

    extension(MethodDefinition element)
    {
        public TO Accept<TO>(IVisitor<MethodDefinition, TO> visitor) => visitor.Visit(element);
    }

    extension(MethodReturnType element)
    {
        public TO Accept<TO>(IVisitor<MethodReturnType, TO> visitor) => visitor.Visit(element);
    }
}
