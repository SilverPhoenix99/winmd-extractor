using System.Collections.Immutable;
using Mono.Cecil;

namespace Winmd.ClassExtensions;

internal static class TypeReferenceExtensions
{
    extension(TypeReference type)
    {
        public TO Accept<TO>(IVisitor<TypeReference, TO> visitor) => visitor.Visit(type);

        public string? GetNamespace()
        {
            var @namespace = type.IsNested ? type.GetNesting()![0].Namespace : type.Namespace;
            return @namespace != "System" ? @namespace : null;
        }

        public IImmutableList<TypeReference>? GetNesting()
        {
            var nesting = GetNestingEnumerable(type).Reverse().ToImmutableList();
            return nesting.IsEmpty ? null : nesting;
        }
    }

    extension(TypeReference? type)
    {
        public (string Name, string? Namespace) GetQualifiedName(bool stripAttributes = true)
        {
            if (type is null)
            {
                return ("", null);
            }

            var name = type.Name;
            if (stripAttributes)
            {
                name = name.StripEnd("Attribute");
            }

            return (name, type.GetNamespace());
        }
    }

    private static IEnumerable<TypeReference> GetNestingEnumerable(TypeReference type)
    {
        for (; type.IsNested ; type = type.DeclaringType)
        {
            yield return type.DeclaringType;
        }
    }
}
